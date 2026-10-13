using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Columns.Commands.CreateColumn;
using TaskFlow.Domain.Entities;
using TaskFlow.UnitTests.Common.TestHelpers;

namespace TaskFlow.UnitTests.Features.Columns.Handlers;

public class CreateColumnCommandHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Guid _userId = Guid.NewGuid();

    public CreateColumnCommandHandlerTests()
    {
        _currentUserMock.Setup(x => x.UserId).Returns(_userId);
    }

    private async Task<(Infrastructure.Persistence.ApplicationDbContext db, Guid projectId)> SeedProjectAsync()
    {
        var db = TestDbContextFactory.Create();

        var project = new Project
        {
            Name = "Test Project",
            OwnerId = _userId,
            CreatedAt = DateTime.UtcNow
        };
        db.Projects.Add(project);
        db.ProjectMembers.Add(new ProjectMember
        {
            UserId = _userId,
            ProjectId = project.Id,
            Role = ProjectRole.Owner,
            JoinedAt = DateTime.UtcNow
        });
        db.BoardColumns.Add(new BoardColumn
        {
            Name = "To Do",
            Position = 1,
            ProjectId = project.Id
        });
        await db.SaveChangesAsync();

        return (db, project.Id);
    }

    [Fact]
    public async Task Should_Create_Column_At_End_When_Position_Not_Specified()
    {
        // Arrange
        var (db, projectId) = await SeedProjectAsync();
        var handler = new CreateColumnCommandHandler(db, _currentUserMock.Object);
        var command = new CreateColumnCommand(projectId, "Review");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Review");
        result.Position.Should().Be(2); // To Do ya existe con pos 1
        result.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public async Task Should_Throw_When_Member_Not_Owner_Or_Admin()
    {
        // Arrange
        var (db, projectId) = await SeedProjectAsync();

        // Convertir al usuario en Member (sin permisos)
        var member = await db.ProjectMembers.FirstAsync(pm => pm.ProjectId == projectId);
        member.Role = ProjectRole.Member;
        await db.SaveChangesAsync();

        var handler = new CreateColumnCommandHandler(db, _currentUserMock.Object);
        var command = new CreateColumnCommand(projectId, "Review");

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Owners and Admins*");
    }
}