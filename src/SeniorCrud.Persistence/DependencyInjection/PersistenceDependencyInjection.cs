using Microsoft.EntityFrameworkCore;
using SeniorCrud.Application.Interfaces.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeniorCrud.Persistence.Contexts;
using SeniorCrud.Persistence.Interceptors;
using SeniorCrud.Persistence.Repositories;
using SeniorCrud.Persistence.Seed;
using SeniorCrud.Persistence.UnitOfWork;

namespace SeniorCrud.Persistence.DependencyInjection;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddScoped<UpdateTimestampsSaveChangesInterceptor>();

        services.AddDbContext<SeniorCrudDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
            }

            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(SeniorCrudDbContext).Assembly.FullName);
                sql.EnableRetryOnFailure();
            });

            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>(),
                serviceProvider.GetRequiredService<UpdateTimestampsSaveChangesInterceptor>());
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IPersistenceSeeder, PersistenceSeeder>();

        return services;
    }
}
