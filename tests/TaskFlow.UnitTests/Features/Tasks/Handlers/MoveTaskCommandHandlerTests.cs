using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Commands.MoveTask;
using TaskFlow.Domain.Entities;
using TaskFlow.UnitTests.Common.TestHelpers;

namespace TaskFlow.UnitTests.Features.Tasks.Handlers;

public class MoveTaskCommandHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Mock<IRealTimeNotifier> _notifierMock = new();
    private readonly Guid _userId = Guid.NewGuid();

    public MoveTaskCommandHandlerTests()
    {
        _currentUserMock.Setup(x => x.UserId).Returns(_userId);
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
    }

    // ============================================
    // HELPER: crea un proyecto con columnas y tareas
    // ============================================
    private async Task<(Infrastructure.Persistence.ApplicationDbContext db, Guid projectId, Guid todoColumnId, Guid inProgressColumnId, Guid taskId)>
        SeedScenarioAsync(int tasksInTodo = 3)
    {
        var db = TestDbContextFactory.Create();

        var project = new Project
        {
            Name = "Test Project",
            OwnerId = _userId,
            CreatedAt = DateTime.UtcNow
        };
        db.Projects.Add(project);

        // Miembro con rol Owner
        db.ProjectMembers.Add(new ProjectMember
        {
            UserId = _userId,
            ProjectId = project.Id,
            Role = ProjectRole.Owner,
            JoinedAt = DateTime.UtcNow
        });

        var todoColumn = new BoardColumn
        {
            Name = "To Do",
            Position = 1,
            ProjectId = project.Id
        };
        var inProgressColumn = new BoardColumn
        {
            Name = "In Progress",
            Position = 2,
            ProjectId = project.Id
        };
        db.BoardColumns.AddRange(todoColumn, inProgressColumn);

        // Crear N tareas en "To Do"
        var tasks = new List<TaskItem>();
        for (int i = 1; i <= tasksInTodo; i++)
        {
            tasks.Add(new TaskItem
            {
                Title = $"Task {i}",
                Position = i,
                ColumnId = todoColumn.Id,
                Priority = TaskPriority.Medium
            });
        }
        db.TaskItems.AddRange(tasks);
        await db.SaveChangesAsync();

        return (db, project.Id, todoColumn.Id, inProgressColumn.Id, tasks[0].Id);
    }

    // ============================================
    // TEST 1: Mover tarea a otra columna
    // ============================================
    [Fact]
    public async Task Should_Move_Task_To_Target_Column_When_Target_Is_Different()
    {
        // Arrange
        var (db, _, _, inProgressColumnId, taskId) = await SeedScenarioAsync();
        var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);

        var command = new MoveTaskCommand(taskId, inProgressColumnId, Position: null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ColumnId.Should().Be(inProgressColumnId);
        result.ColumnName.Should().Be("In Progress");

        var task = await db.TaskItems.FindAsync(taskId);
        task!.ColumnId.Should().Be(inProgressColumnId);
        task.Position.Should().Be(1); // primera posición en la nueva columna
    }

    // ============================================
    // TEST 2: Reordenar dentro de la misma columna
    // ============================================
    [Fact]
    public async Task Should_Reorder_Task_Within_Same_Column_When_Position_Changes()
    {
        // Arrange
        var (db, _, todoColumnId, _, taskId) = await SeedScenarioAsync(tasksInTodo: 3);

        // Tomar la última tarea (Task 3) y moverla a la primera posición
        var lastTask = await db.TaskItems
            .Where(t => t.ColumnId == todoColumnId)
            .OrderByDescending(t => t.Position)
            .FirstAsync();

        var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);
        var command = new MoveTaskCommand(lastTask.Id, todoColumnId, Position: 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Position.Should().Be(1);

        // Verificar el orden final en la BD
        var tasks = await db.TaskItems
            .Where(t => t.ColumnId == todoColumnId)
            .OrderBy(t => t.Position)
            .ToListAsync();

        tasks[0].Id.Should().Be(lastTask.Id);      // la que movimos está en posición 1
        tasks.Select(t => t.Position).Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    // ============================================
    // TEST 3: Debe fallar si la tarea no existe
    // ============================================
    [Fact]
    public async Task Should_Throw_When_Task_Does_Not_Exist()
    {
        // Arrange
        var (db, _, _, inProgressColumnId, _) = await SeedScenarioAsync();
        var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);

        var command = new MoveTaskCommand(Guid.NewGuid(), inProgressColumnId, null);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*not found*");
    }

    // ============================================
    // TEST 4: Debe fallar si la columna destino no existe
    // ============================================
    [Fact]
    public async Task Should_Throw_When_Target_Column_Does_Not_Exist()
    {
        // Arrange
        var (db, _, _, _, taskId) = await SeedScenarioAsync();
        var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);

        var command = new MoveTaskCommand(taskId, Guid.NewGuid(), null);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*column*not found*");
    }

    // ============================================
    // TEST 5: Debe fallar si el usuario no es miembro del proyecto
    // ============================================
    [Fact]
    public async Task Should_Throw_When_User_Is_Not_Member_Of_Project()
    {
        // Arrange
        var (db, _, _, inProgressColumnId, taskId) = await SeedScenarioAsync();

        // Otro usuario, sin acceso al proyecto
        var otherUser = Guid.NewGuid();
        _currentUserMock.Setup(x => x.UserId).Returns(otherUser);

        var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);
        var command = new MoveTaskCommand(taskId, inProgressColumnId, null);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*do not have access*");
    }

    // ============================================
    // TEST 6: Debe notificar por SignalR cuando se mueve
    // ============================================
    [Fact]
    public async Task Should_Notify_Realtime_When_Task_Is_Moved()
    {
        // Arrange
        var (db, _, _, inProgressColumnId, taskId) = await SeedScenarioAsync();
        var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);
        var command = new MoveTaskCommand(taskId, inProgressColumnId, null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _notifierMock.Verify(
            x => x.NotifyTaskMovedAsync(
                It.IsAny<Guid>(),
                It.IsAny<object>()),
            Times.Once);
    }
}