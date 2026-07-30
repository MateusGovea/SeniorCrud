namespace SeniorCrud.Domain.Exceptions;

public sealed class InvalidCepException : DomainException
{
    public InvalidCepException(string? value)
        : base($"Invalid CEP: '{value ?? "null"}'.")
    {
    }
}
