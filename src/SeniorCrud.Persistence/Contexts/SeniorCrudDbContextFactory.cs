using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SeniorCrud.Persistence.Contexts;

public sealed class SeniorCrudDbContextFactory : IDesignTimeDbContextFactory<SeniorCrudDbContext>
{
    private const string FallbackConnectionString = "Server=localhost,1433;Database=SeniorCrudDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";

    public SeniorCrudDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SeniorCrudDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("SENIORCRUD_CONNECTION_STRING");
        connectionString = string.IsNullOrWhiteSpace(connectionString)
            ? FallbackConnectionString
            : connectionString;

        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(SeniorCrudDbContext).Assembly.FullName);
        });

        return new SeniorCrudDbContext(optionsBuilder.Options);
    }
}
