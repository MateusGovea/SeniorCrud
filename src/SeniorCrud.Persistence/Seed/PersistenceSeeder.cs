using Microsoft.EntityFrameworkCore;
using SeniorCrud.Persistence.Contexts;

namespace SeniorCrud.Persistence.Seed;

public sealed class PersistenceSeeder : IPersistenceSeeder
{
    private readonly SeniorCrudDbContext _dbContext;

    public PersistenceSeeder(SeniorCrudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);
    }
}
