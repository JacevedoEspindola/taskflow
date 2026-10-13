using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.Commands.CreateProject;
using TaskFlow.Domain.Entities;
using TaskFlow.UnitTests.Common.TestHelpers;

namespace TaskFlow.UnitTests.Features.Projects.Handlers;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Guid _userId = Guid.NewGuid();

    public CreateProjectCommandHandlerTests()
    {
        _currentUserMock.Setup(x => x.UserId).Returns(_userId);
    }

    [Fact]
    public async Task Should_Create_Project_With_Default_Columns_And_Owner_Member()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();
        var handler = new CreateProjectCommandHandler(db, _currentUserMock.Object);

        var command = new CreateProjectCommand("My Project", "Some description");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("My Project");
        result.Description.Should().Be("Some description");
        result.OwnerId.Should().Be(_userId);
        result.Columns.Should().HaveCount(3);
        result.Columns.Select(c => c.Name).Should().BeEquivalentTo(
            new[] { "To Do", "In Progress", "Done" });

        // Verificar en la BD
        var project = await db.Projects.FirstAsync(p => p.Id == result.Id);
        project.Name.Should().Be("My Project");

        var members = await db.ProjectMembers
            .Where(pm => pm.ProjectId == project.Id)
            .ToListAsync();
        members.Should().HaveCount(1);
        members[0].Role.Should().Be(ProjectRole.Owner);
        members[0].UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task Should_Throw_When_User_Is_Not_Authenticated()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();
        _currentUserMock.Setup(x => x.UserId).Returns((Guid?)null);

        var handler = new CreateProjectCommandHandler(db, _currentUserMock.Object);
        var command = new CreateProjectCommand("Test", null);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*logged in*");
    }

    [Fact]
    public async Task Should_Trim_Name_And_Description()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();
        var handler = new CreateProjectCommandHandler(db, _currentUserMock.Object);
        var command = new CreateProjectCommand("  Padded Name  ", "  Padded description  ");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Padded Name");
        result.Description.Should().Be("Padded description");
    }
}