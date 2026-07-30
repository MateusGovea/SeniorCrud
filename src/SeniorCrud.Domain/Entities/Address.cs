using SeniorCrud.Domain.Common;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Exceptions;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Domain.Entities;

public sealed class Address : AuditableEntity
{
    public Guid UserId { get; private set; }

    public Cep Cep { get; private set; } = null!;

    public string Street { get; private set; } = string.Empty;

    public AddressNumber Number { get; private set; } = null!;

    public string? Complement { get; private set; }

    public string Neighborhood { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public string State { get; private set; } = string.Empty;

    public bool IsPrimary { get; private set; }

    public Address(
        Guid id,
        Guid userId,
        Cep cep,
        string street,
        AddressNumber number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        bool isPrimary,
        DateTimeOffset createdAtUtc)
        : base(id, createdAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("UserId cannot be empty.");
        }

        UserId = userId;
        Cep = cep ?? throw new InvalidCepException(null);
        Street = ValidateStreet(street);
        Number = number;
        Complement = ValidateComplement(complement);
        Neighborhood = ValidateNeighborhood(neighborhood);
        City = ValidateCity(city);
        State = ValidateState(state);
        IsPrimary = isPrimary;
    }

    private Address()
    {
    }

    public void Update(
        Cep cep,
        string street,
        AddressNumber number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        DateTimeOffset updatedAtUtc)
    {
        Cep = cep ?? throw new InvalidCepException(null);
        Street = ValidateStreet(street);
        Number = number;
        Complement = ValidateComplement(complement);
        Neighborhood = ValidateNeighborhood(neighborhood);
        City = ValidateCity(city);
        State = ValidateState(state);

        MarkUpdated(updatedAtUtc);
    }

    public void SetAsPrimary(DateTimeOffset updatedAtUtc)
    {
        IsPrimary = true;
        MarkUpdated(updatedAtUtc);
    }

    public void SetAsSecondary(DateTimeOffset updatedAtUtc)
    {
        IsPrimary = false;
        MarkUpdated(updatedAtUtc);
    }

    private static string ValidateStreet(string value)
    {
        var normalized = Normalize(value);

        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > DomainConstraints.MaxStreetLength)
        {
            throw new DomainException("Street is invalid.");
        }

        return normalized;
    }

    private static string ValidateNeighborhood(string value)
    {
        var normalized = Normalize(value);

        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > DomainConstraints.MaxNeighborhoodLength)
        {
            throw new DomainException("Neighborhood is invalid.");
        }

        return normalized;
    }

    private static string ValidateCity(string value)
    {
        var normalized = Normalize(value);

        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > DomainConstraints.MaxCityLength)
        {
            throw new DomainException("City is invalid.");
        }

        return normalized;
    }

    private static string ValidateState(string value)
    {
        var normalized = Normalize(value).ToUpperInvariant();

        if (normalized.Length != DomainConstraints.StateLength || !normalized.All(char.IsLetter))
        {
            throw new DomainException("State is invalid.");
        }

        return normalized;
    }

    private static string? ValidateComplement(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var normalized = Normalize(value);
        if (normalized.Length > DomainConstraints.MaxComplementLength)
        {
            throw new DomainException("Complement is invalid.");
        }

        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string Normalize(string value)
    {
        return (value ?? string.Empty).Trim();
    }
}
