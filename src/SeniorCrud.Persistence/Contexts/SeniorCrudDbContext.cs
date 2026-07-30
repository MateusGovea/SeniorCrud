using Microsoft.EntityFrameworkCore;
using SeniorCrud.Domain.Entities;

namespace SeniorCrud.Persistence.Contexts;

public sealed class SeniorCrudDbContext : DbContext
{
    public const string DefaultSchema = "core";

    public SeniorCrudDbContext(DbContextOptions<SeniorCrudDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeniorCrudDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
