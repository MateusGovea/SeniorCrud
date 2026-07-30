namespace SeniorCrud.Application.Results;

public sealed record ValidationError(string PropertyName, string ErrorMessage);
