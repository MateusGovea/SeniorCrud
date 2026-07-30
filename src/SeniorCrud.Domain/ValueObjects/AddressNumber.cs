using SeniorCrud.Domain.Common;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Exceptions;

namespace SeniorCrud.Domain.ValueObjects;

public sealed class AddressNumber : ValueObject
{
    public string Value { get; }

    public AddressNumber(string value)
    {
        var normalized = Normalize(value);

        if (!IsValid(normalized))
        {
            throw new InvalidAddressNumberException(value);
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
        return (value ?? string.Empty).Trim().ToUpperInvariant();
    }

    private static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length <= DomainConstraints.MaxAddressNumberLength;
    }
}
