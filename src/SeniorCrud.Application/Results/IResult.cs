namespace SeniorCrud.Application.Results;

public interface IResult
{
    bool IsSuccess { get; }

    Error Error { get; }

    IReadOnlyCollection<ValidationError> ValidationErrors { get; }
}
