using FluentAssertions;
using SeniorCrud.Application.Results;

namespace SeniorCrud.UnitTests.Application;

public class ResultPatternTests
{
    [Fact]
    public void Result_Success_Should_Set_IsSuccess()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Result_Failure_Should_Set_Error()
    {
        var error = new Error("X", "failure", ErrorType.Failure);
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Result_ValidationFailure_Should_Store_Errors()
    {
        var result = Result.ValidationFailure([
            new ValidationError("Field", "Required")
        ]);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.ValidationErrors.Should().HaveCount(1);
    }

    [Fact]
    public void ResultOfT_Success_Should_Expose_Value()
    {
        var result = Result<string>.Success("ok");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public void ResultFactory_Should_Create_Generic_Failure()
    {
        var created = ResultFactory.CreateFailure<Result<string>>(new Error("C", "desc", ErrorType.Conflict));

        created.IsSuccess.Should().BeFalse();
        created.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void Result_Should_Throw_When_Invalid_Success_Error_Combination()
    {
        var action = () => new TestableResult(true, new Error("E", "desc", ErrorType.Failure));

        action.Should().Throw<InvalidOperationException>();
    }

    private sealed class TestableResult : Result
    {
        public TestableResult(bool isSuccess, Error error)
            : base(isSuccess, error)
        {
        }
    }
}
