namespace SeniorCrud.Domain.Exceptions;

public sealed class InvalidEmailException : DomainException
{
    public InvalidEmailException(string? value)
        : base($"Invalid email: '{value ?? "null"}'.")
    {
    }
}
