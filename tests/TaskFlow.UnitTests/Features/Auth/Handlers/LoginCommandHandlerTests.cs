using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Moq;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.Login;
using TaskFlow.Domain.Entities;
using TaskFlow.UnitTests.Common.TestHelpers;

namespace TaskFlow.UnitTests.Features.Auth.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();

    public LoginCommandHandlerTests()
    {
        _jwtServiceMock
            .Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");
    }

    [Fact]
    public async Task Should_Return_Token_When_Credentials_Are_Valid()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();
        db.Users.Add(new User
        {
            Email = "user@test.com",
            FullName = "User Test",
            PasswordHash = "hashed-Password123"
        });
        await db.SaveChangesAsync();

        _passwordHasherMock
            .Setup(x => x.Verify("Password123", "hashed-Password123"))
            .Returns(true);

        var handler = new LoginCommandHandler(db, _passwordHasherMock.Object, _jwtServiceMock.Object);
        var command = new LoginCommand("user@test.com", "Password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Email.Should().Be("user@test.com");
        result.Token.Should().Be("fake-jwt-token");
    }

    [Fact]
    public async Task Should_Throw_When_User_Not_Found()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();
        var handler = new LoginCommandHandler(db, _passwordHasherMock.Object, _jwtServiceMock.Object);
        var command = new LoginCommand("nobody@test.com", "Password123");

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid credentials*");
    }

    [Fact]
    public async Task Should_Throw_When_Password_Is_Incorrect()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();
        db.Users.Add(new User
        {
            Email = "user@test.com",
            FullName = "User",
            PasswordHash = "hashed-correct"
        });
        await db.SaveChangesAsync();

        _passwordHasherMock
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var handler = new LoginCommandHandler(db, _passwordHasherMock.Object, _jwtServiceMock.Object);
        var command = new LoginCommand("user@test.com", "WrongPassword");

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid credentials*");
    }
}