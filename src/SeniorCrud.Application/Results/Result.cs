namespace SeniorCrud.Application.Results;

public class Result : IResult
{
    private readonly List<ValidationError> _validationErrors;

    protected internal Result(bool isSuccess, Error error, IEnumerable<ValidationError>? validationErrors = null)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Successful results cannot contain errors.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Failure results must contain an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
        _validationErrors = validationErrors?.ToList() ?? [];
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public IReadOnlyCollection<ValidationError> ValidationErrors => _validationErrors.AsReadOnly();

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result Failure(string code, string description) => Failure(new Error(code, description, ErrorType.Failure));

    public static Result ValidationFailure(IEnumerable<ValidationError> validationErrors)
        => new(false, new Error("Validation.Failed", "One or more validation errors have occurred.", ErrorType.Validation), validationErrors);

    public static Result NotFound(string code, string description)
        => Failure(new Error(code, description, ErrorType.NotFound));

    public static Result Conflict(string code, string description)
        => Failure(new Error(code, description, ErrorType.Conflict));

    public static Result Unauthorized(string code, string description)
        => Failure(new Error(code, description, ErrorType.Unauthorized));

    public static Result Forbidden(string code, string description)
        => Failure(new Error(code, description, ErrorType.Forbidden));
}
