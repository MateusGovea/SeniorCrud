using Microsoft.Extensions.DependencyInjection;

namespace SeniorCrud.Persistence.DependencyInjection;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        return services;
    }
}
