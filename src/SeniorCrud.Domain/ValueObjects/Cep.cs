using SeniorCrud.Domain.Common;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Exceptions;

namespace SeniorCrud.Domain.ValueObjects;

public sealed class Cep : ValueObject
{
    public string Value { get; }

    public Cep(string value)
    {
        var normalized = Normalize(value);

        if (!IsValid(normalized))
        {
            throw new InvalidCepException(value);
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
        return new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    private static bool IsValid(string value)
    {
        return value.Length == DomainConstraints.CepLength && value.All(char.IsDigit);
    }
}
