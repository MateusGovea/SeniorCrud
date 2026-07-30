using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SeniorCrud.Application.Behaviors;
using SeniorCrud.Application.Features.Authentication.Commands;
using SeniorCrud.Application.Results;

namespace SeniorCrud.UnitTests.Application;

public class BehaviorsTests
{
    [Fact]
    public async Task ValidationBehavior_Should_Call_Next_When_No_Validators()
    {
        var behavior = new ValidationBehavior<LoginCommand, Result<string>>([]);
        var nextCalled = false;

        var result = await behavior.Handle(
            new LoginCommand("user@test.com", "password123"),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult(Result<string>.Success("ok"));
            },
            CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidationBehavior_Should_Return_Validation_Failure_When_Invalid()
    {
        var validator = new Mock<IValidator<LoginCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([
                new ValidationFailure("Email", "invalid")
            ]));

        var behavior = new ValidationBehavior<LoginCommand, Result<string>>([validator.Object]);

        var result = await behavior.Handle(
            new LoginCommand("bad", "x"),
            _ => Task.FromResult(Result<string>.Success("ok")),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.ValidationErrors.Should().ContainSingle(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task ExceptionBehavior_Should_Catch_Exception_And_Return_Failure()
    {
        var logger = new Mock<ILogger<ExceptionBehavior<LoginCommand, Result<string>>>>();
        var behavior = new ExceptionBehavior<LoginCommand, Result<string>>(logger.Object);

        var result = await behavior.Handle(
            new LoginCommand("user@test.com", "password"),
            _ => throw new InvalidOperationException("boom"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Application.Unhandled");
    }

    [Fact]
    public async Task LoggingBehavior_Should_Return_Result_From_Next()
    {
        var logger = new Mock<ILogger<LoggingBehavior<LoginCommand, Result<string>>>>();
        var behavior = new LoggingBehavior<LoginCommand, Result<string>>(logger.Object);

        var result = await behavior.Handle(
            new LoginCommand("user@test.com", "password"),
            _ => Task.FromResult(Result<string>.Success("ok")),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public async Task PerformanceBehavior_Should_Return_Result_From_Next()
    {
        var logger = new Mock<ILogger<PerformanceBehavior<LoginCommand, Result<string>>>>();
        var behavior = new PerformanceBehavior<LoginCommand, Result<string>>(logger.Object);

        var result = await behavior.Handle(
            new LoginCommand("user@test.com", "password"),
            _ => Task.FromResult(Result<string>.Success("ok")),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
