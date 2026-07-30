namespace SeniorCrud.Application.Results;

public sealed class Result<TValue> : Result
{
    private Result(TValue? value, bool isSuccess, Error error, IEnumerable<ValidationError>? validationErrors = null)
        : base(isSuccess, error, validationErrors)
    {
        Value = value;
    }

    public TValue? Value { get; }

    public static Result<TValue> Success(TValue value)
        => new(value, true, Error.None);

    public new static Result<TValue> Failure(Error error)
        => new(default, false, error);

    public new static Result<TValue> Failure(string code, string description)
        => Failure(new Error(code, description, ErrorType.Failure));

    public new static Result<TValue> ValidationFailure(IEnumerable<ValidationError> validationErrors)
        => new(default, false, new Error("Validation.Failed", "One or more validation errors have occurred.", ErrorType.Validation), validationErrors);

    public new static Result<TValue> NotFound(string code, string description)
        => Failure(new Error(code, description, ErrorType.NotFound));

    public new static Result<TValue> Conflict(string code, string description)
        => Failure(new Error(code, description, ErrorType.Conflict));

    public new static Result<TValue> Unauthorized(string code, string description)
        => Failure(new Error(code, description, ErrorType.Unauthorized));

    public new static Result<TValue> Forbidden(string code, string description)
        => Failure(new Error(code, description, ErrorType.Forbidden));
}
