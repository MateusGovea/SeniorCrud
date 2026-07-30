namespace SeniorCrud.Domain.Exceptions;

public sealed class InvalidUserStateException : DomainException
{
    public InvalidUserStateException(string message)
        : base(message)
    {
    }
}
