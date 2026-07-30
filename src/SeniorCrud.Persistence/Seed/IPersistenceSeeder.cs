namespace SeniorCrud.Persistence.Seed;

public interface IPersistenceSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
