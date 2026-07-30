using SeniorCrud.Domain.Common;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Exceptions;

namespace SeniorCrud.Domain.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    public PasswordHash(string value)
    {
        var normalized = (value ?? string.Empty).Trim();

        if (normalized.Length < DomainConstraints.MinPasswordHashLength ||
            normalized.Length > DomainConstraints.MaxPasswordHashLength)
        {
            throw new InvalidPasswordHashException();
        }

        Value = normalized;
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
