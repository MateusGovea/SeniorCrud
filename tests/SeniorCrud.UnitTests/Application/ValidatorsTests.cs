using FluentAssertions;
using SeniorCrud.Application.Features.Addresses.Commands;
using SeniorCrud.Application.Features.Authentication.Commands;
using SeniorCrud.Application.Features.Users.Commands;
using SeniorCrud.Application.Validators.Addresses;
using SeniorCrud.Application.Validators.Authentication;
using SeniorCrud.Application.Validators.Users;

namespace SeniorCrud.UnitTests.Application;

public class ValidatorsTests
{
    [Fact]
    public void LoginCommandValidator_Should_Validate_Valid_Command()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("user@test.com", "password123");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void LoginCommandValidator_Should_Fail_When_Email_Invalid()
    {
        var validator = new LoginCommandValidator();

        var result = validator.Validate(new LoginCommand("invalid", "password123"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateUserCommandValidator_Should_Fail_When_Name_Too_Short()
    {
        var validator = new CreateUserCommandValidator();
        var command = new CreateUserCommand("ab", "user@test.com", "password123", "39053344705", null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateUserCommandValidator_Should_Fail_When_UserId_Empty()
    {
        var validator = new UpdateUserCommandValidator();
        var command = new UpdateUserCommand(Guid.Empty, "Name", "user@test.com", null, null, true);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateAddressCommandValidator_Should_Validate_Valid_Command()
    {
        var validator = new CreateAddressCommandValidator();
        var command = new CreateAddressCommand(
            Guid.NewGuid(),
            "01001000",
            "Rua A",
            "10",
            null,
            "Centro",
            "Sao Paulo",
            "SP",
            true);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateAddressCommandValidator_Should_Fail_When_State_Invalid()
    {
        var validator = new UpdateAddressCommandValidator();
        var command = new UpdateAddressCommand(
            Guid.NewGuid(),
            "01001000",
            "Rua A",
            "10",
            null,
            "Centro",
            "Sao Paulo",
            "S1",
            true);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
