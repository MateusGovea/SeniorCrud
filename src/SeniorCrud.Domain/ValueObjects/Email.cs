using System.Net.Mail;
using SeniorCrud.Domain.Common;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Exceptions;

namespace SeniorCrud.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        var normalized = Normalize(value);

        if (!IsValid(normalized))
        {
            throw new InvalidEmailException(value);
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

    private static string Normalize(string value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > DomainConstraints.MaxEmailLength)
        {
            return false;
        }

        try
        {
            _ = new MailAddress(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
