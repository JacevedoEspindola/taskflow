using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.Register;
using TaskFlow.Application.Features.Auth.Dtos;
using TaskFlow.Domain.Entities;
using TaskFlow.UnitTests.Common.TestHelpers;

namespace TaskFlow.UnitTests.Features.Auth.Handlers;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();

    public RegisterCommandHandlerTests()
    {
        // Configuración por defecto de los mocks
        _passwordHasherMock
            .Setup(x => x.Hash(It.IsAny<string>()))
            .Returns<string>(p => $"hashed-{p}");

        _jwtServiceMock
            .Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");
    }

    [Fact]
    public async Task Should_Register_User_When_Email_Is_New()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();

        var handler = new RegisterCommandHandler(
            db,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object);

        var command = new RegisterCommand(
            Email: "newuser@test.com",
            Password: "Password123",
            FullName: "New User");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("newuser@test.com");
        result.FullName.Should().Be("New User");
        result.Token.Should().Be("fake-jwt-token");

        // Verificar en la BD
        var savedUser = await db.Users.FirstOrDefaultAsync(u => u.Email == "newuser@test.com");
        savedUser.Should().NotBeNull();
        savedUser!.PasswordHash.Should().Be("hashed-Password123");
    }

    [Fact]
    public async Task Should_Throw_When_Email_Already_Exists()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();

        // Sembrar un usuario existente
        db.Users.Add(new User
        {
            Email = "existing@test.com",
            FullName = "Existing User",
            PasswordHash = "hashed-something"
        });
        await db.SaveChangesAsync();

        var handler = new RegisterCommandHandler(
            db,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object);

        var command = new RegisterCommand(
            Email: "existing@test.com",
            Password: "Password123",
            FullName: "Another User");

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already*"); // o "*ya está registrado*" según tu mensaje
    }

    [Fact]
    public async Task Should_Normalize_Email_To_Lowercase()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();
        var handler = new RegisterCommandHandler(db, _passwordHasherMock.Object, _jwtServiceMock.Object);

        var command = new RegisterCommand(
            Email: "MiXeDcAsE@Test.COM",
            Password: "Password123",
            FullName: "Test");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Email.Should().Be("mixedcase@test.com");
    }

    [Fact]
    public async Task Should_Not_Save_User_When_Email_Exists()
    {
        // Arrange
        await using var db = TestDbContextFactory.Create();

        db.Users.Add(new User
        {
            Email = "existing@test.com",
            FullName = "Existing",
            PasswordHash = "hash"
        });
        await db.SaveChangesAsync();

        var handler = new RegisterCommandHandler(db, _passwordHasherMock.Object, _jwtServiceMock.Object);
        var command = new RegisterCommand("existing@test.com", "Password123", "Another");

        // Act
        try { await handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        var userCount = await db.Users.CountAsync();
        userCount.Should().Be(1); // no se agregó otro
    }
}