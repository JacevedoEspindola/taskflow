using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using TaskFlow.Application.Features.Auth.Commands.Register;

namespace TaskFlow.UnitTests.Features.Auth.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "user@test.com",
            Password: "Password123",
            FullName: "Test User");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Password123", "Test", "Email")]
    [InlineData("invalid-email", "Password123", "Test", "Email")]
    [InlineData("user@test.com", "", "Test", "Password")]
    [InlineData("user@test.com", "123", "Test", "Password")]
    [InlineData("user@test.com", "password123", "Test", "Password")]
    [InlineData("user@test.com", "PASSWORD123", "Test", "Password")]
    [InlineData("user@test.com", "Password", "Test", "Password")]
    [InlineData("user@test.com", "Password123", "", "FullName")]
    public void Should_Fail_When_Field_Is_Invalid(
        string email, string password, string fullName, string expectedInvalidProperty)
    {
        // Arrange
        var command = new RegisterCommand(email, password, fullName);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedInvalidProperty);
    }
}