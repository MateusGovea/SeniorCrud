namespace SeniorCrud.Domain.Exceptions;

public sealed class InvalidAddressNumberException : DomainException
{
    public InvalidAddressNumberException(string? value)
        : base($"Invalid address number: '{value ?? "null"}'.")
    {
    }
}
