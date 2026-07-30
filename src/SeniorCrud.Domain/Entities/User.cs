using SeniorCrud.Domain.Common;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.Exceptions;
using SeniorCrud.Domain.Interfaces;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Domain.Entities;

public sealed class User : AuditableEntity, IAggregateRoot
{
    private readonly List<Address> _addresses = [];

    public string Name { get; private set; } = string.Empty;

    public Email Email { get; private set; } = null!;

    public Cpf? Cpf { get; private set; }

    public DateOnly? BirthDate { get; private set; }

    public PasswordHash PasswordHash { get; private set; } = null!;

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    public User(
        Guid id,
        string name,
        Email email,
        PasswordHash passwordHash,
        UserRole role,
        DateTimeOffset createdAtUtc,
        Cpf? cpf = null,
        DateOnly? birthDate = null)
        : base(id, createdAtUtc)
    {
        Name = ValidateName(name);
        Email = email ?? throw new InvalidEmailException(null);
        PasswordHash = passwordHash;
        Role = ValidateRole(role);
        IsActive = true;
        Cpf = cpf;
        BirthDate = ValidateBirthDate(birthDate);
    }

    private User()
    {
    }

    public void UpdateProfile(
        string name,
        Email email,
        Cpf? cpf,
        DateOnly? birthDate,
        DateTimeOffset updatedAtUtc)
    {
        Name = ValidateName(name);
        Email = email ?? throw new InvalidEmailException(null);
        Cpf = cpf;
        BirthDate = ValidateBirthDate(birthDate);

        MarkUpdated(updatedAtUtc);
    }

    public void ChangePassword(PasswordHash newPasswordHash, DateTimeOffset updatedAtUtc)
    {
        PasswordHash = newPasswordHash;
        MarkUpdated(updatedAtUtc);
    }

    public void ChangeRole(UserRole role, DateTimeOffset updatedAtUtc)
    {
        Role = ValidateRole(role);
        MarkUpdated(updatedAtUtc);
    }

    public void Activate(DateTimeOffset updatedAtUtc)
    {
        IsActive = true;
        MarkUpdated(updatedAtUtc);
    }

    public void Deactivate(DateTimeOffset updatedAtUtc)
    {
        IsActive = false;
        MarkUpdated(updatedAtUtc);
    }

    public void EnsureCanAuthenticate()
    {
        if (!IsActive)
        {
            throw new InvalidUserStateException("Inactive users cannot authenticate.");
        }
    }

    public void AddAddress(Address address, DateTimeOffset updatedAtUtc)
    {
        if (address is null)
        {
            throw new DomainException("Address cannot be null.");
        }

        if (address.UserId != Id)
        {
            throw new DomainException("Address does not belong to this user.");
        }

        if (_addresses.Any(a => a.Id == address.Id))
        {
            throw new DomainException("Address already exists for this user.");
        }

        if (address.IsPrimary)
        {
            UnsetPrimaryAddress(updatedAtUtc);
        }

        _addresses.Add(address);
        MarkUpdated(updatedAtUtc);
    }

    public void RemoveAddress(Guid addressId, DateTimeOffset updatedAtUtc)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null)
        {
            throw new DomainException("Address was not found for this user.");
        }

        _addresses.Remove(address);
        MarkUpdated(updatedAtUtc);
    }

    public void SetPrimaryAddress(Guid addressId, DateTimeOffset updatedAtUtc)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null)
        {
            throw new DomainException("Address was not found for this user.");
        }

        UnsetPrimaryAddress(updatedAtUtc);
        address.SetAsPrimary(updatedAtUtc);

        MarkUpdated(updatedAtUtc);
    }

    private void UnsetPrimaryAddress(DateTimeOffset updatedAtUtc)
    {
        foreach (var existingPrimary in _addresses.Where(a => a.IsPrimary))
        {
            existingPrimary.SetAsSecondary(updatedAtUtc);
        }
    }

    private static string ValidateName(string value)
    {
        var normalized = (value ?? string.Empty).Trim();

        if (normalized.Length < DomainConstraints.MinUserNameLength ||
            normalized.Length > DomainConstraints.MaxUserNameLength)
        {
            throw new InvalidUserStateException("User name is invalid.");
        }

        return normalized;
    }

    private static UserRole ValidateRole(UserRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new InvalidUserStateException("User role is invalid.");
        }

        return role;
    }

    private static DateOnly? ValidateBirthDate(DateOnly? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (value > today)
        {
            throw new InvalidUserStateException("Birth date cannot be in the future.");
        }

        return value;
    }
}
