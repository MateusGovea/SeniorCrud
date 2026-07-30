using Microsoft.Extensions.DependencyInjection;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Abstractions.Csv;
using SeniorCrud.Application.Abstractions.DateTime;
using SeniorCrud.Application.Abstractions.Identity;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Infrastructure.Authentication;
using SeniorCrud.Infrastructure.Authentication.Options;
using SeniorCrud.Infrastructure.Caching;
using SeniorCrud.Infrastructure.Caching.Options;
using SeniorCrud.Infrastructure.Csv;
using SeniorCrud.Infrastructure.Csv.Options;
using SeniorCrud.Infrastructure.Services;
using SeniorCrud.Infrastructure.ViaCep;
using SeniorCrud.Infrastructure.ViaCep.Options;

namespace SeniorCrud.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateOnStart();

        services.AddOptions<ViaCepOptions>()
            .BindConfiguration(ViaCepOptions.SectionName)
            .ValidateOnStart();

        services.AddOptions<CsvOptions>()
            .BindConfiguration(CsvOptions.SectionName)
            .ValidateOnStart();

        services.AddOptions<CacheOptions>()
            .BindConfiguration(CacheOptions.SectionName)
            .ValidateOnStart();

        services.AddHttpClient<IViaCepClient, ViaCepClient>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICsvExportService, CsvExportService>();
        services.AddScoped<ICacheService, MemoryCacheService>();

        return services;
    }
}
