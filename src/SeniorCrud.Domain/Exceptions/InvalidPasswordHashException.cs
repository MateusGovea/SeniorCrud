namespace SeniorCrud.Domain.Exceptions;

public sealed class InvalidPasswordHashException : DomainException
{
    public InvalidPasswordHashException()
        : base("Password hash is invalid.")
    {
    }
}
