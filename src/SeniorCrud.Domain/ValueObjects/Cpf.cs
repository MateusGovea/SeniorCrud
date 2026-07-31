using SeniorCrud.Domain.Common;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Exceptions;

namespace SeniorCrud.Domain.ValueObjects;

public sealed class Cpf : ValueObject
{
    public string Value { get; }

    public Cpf(string value)
    {
        var normalized = Normalize(value);

        if (!IsValid(normalized))
        {
            throw new InvalidCpfException(value);
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

    private static string Normalize(string? value)
    {
        return new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    public static bool IsValid(string? value)
    {
        var normalized = Normalize(value);

        if (normalized.Length != DomainConstraints.CpfLength)
        {
            return false;
        }

        if (normalized.Distinct().Count() == 1)
        {
            return false;
        }

        var firstDigit = CalculateDigit(normalized.AsSpan(0, 9), 10);
        var secondDigit = CalculateDigit(normalized.AsSpan(0, 10), 11);

        return normalized[9] - '0' == firstDigit && normalized[10] - '0' == secondDigit;
    }

    private static int CalculateDigit(ReadOnlySpan<char> digits, int factor)
    {
        var sum = 0;

        foreach (var digit in digits)
        {
            sum += (digit - '0') * factor;
            factor--;
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
