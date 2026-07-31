using Microsoft.EntityFrameworkCore;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.ValueObjects;
using SeniorCrud.Persistence.Contexts;

namespace SeniorCrud.Persistence.Seed;

public sealed class PersistenceSeeder : IPersistenceSeeder
{
    private readonly SeniorCrudDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public PersistenceSeeder(SeniorCrudDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        await SeedAdminUserAsync(cancellationToken);
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {
        const string adminEmail = "admin@seniorcrud.com";

        if (await _dbContext.Set<User>().AnyAsync(user => user.Email.Value == adminEmail, cancellationToken))
        {
            return;
        }

        var admin = new User(
            Guid.NewGuid(),
            "Administrador",
            new Email(adminEmail),
            new PasswordHash(_passwordHasher.HashPassword("Admin@123")),
            UserRole.Admin,
            DateTimeOffset.UtcNow);

        _dbContext.Set<User>().Add(admin);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
