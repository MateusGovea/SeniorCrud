using AutoMapper;
using SeniorCrud.Application.DependencyInjection;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.UnitTests.Common;

internal static class TestDataFactory
{
    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
            cfg.AddMaps(typeof(ApplicationDependencyInjection).Assembly));
        return configuration.CreateMapper();
    }

    public static User CreateUser(
        Guid? id = null,
        string? name = null,
        string? email = null,
        string? passwordHash = null,
        UserRole role = UserRole.User,
        string? cpf = "390.533.447-05",
        DateOnly? birthDate = null,
        bool isActive = true)
    {
        var now = DateTimeOffset.UtcNow;
        var user = new User(
            id ?? Guid.NewGuid(),
            name ?? "Test User",
            new Email(email ?? "user@test.com"),
            new PasswordHash(passwordHash ?? new string('x', 60)),
            role,
            now,
            string.IsNullOrWhiteSpace(cpf) ? null : new Cpf(cpf),
            birthDate ?? new DateOnly(1990, 1, 1));

        if (!isActive)
        {
            user.Deactivate(now.AddMinutes(1));
        }

        return user;
    }

    public static Address CreateAddress(Guid? id = null, Guid? userId = null, bool isPrimary = false)
    {
        return new Address(
            id ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid(),
            new Cep("01001000"),
            "Rua Teste",
            new AddressNumber("123A"),
            "Apto 5",
            "Centro",
            "Sao Paulo",
            "SP",
            isPrimary,
            DateTimeOffset.UtcNow);
    }
}
