namespace SeniorCrud.Domain.Exceptions;

public sealed class InvalidCpfException : DomainException
{
    public InvalidCpfException(string? value)
        : base($"Invalid CPF: '{value ?? "null"}'.")
    {
    }
}
