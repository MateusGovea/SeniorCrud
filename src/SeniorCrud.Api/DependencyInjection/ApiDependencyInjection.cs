using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using SeniorCrud.Api.ExceptionHandling;
using SeniorCrud.Application.DependencyInjection;
using SeniorCrud.Infrastructure.DependencyInjection;
using SeniorCrud.Persistence.DependencyInjection;

namespace SeniorCrud.Api.DependencyInjection;

public static class ApiDependencyInjection
{
    public static IServiceCollection AddApiFoundation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure();
        services.AddPersistence();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHealthChecks();
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddOptions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddAuthorization();

        services.AddCors(options =>
        {
            options.AddPolicy("Default", policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.AddOpenTelemetry();

        services.AddValidatorsFromAssembly(typeof(ApiDependencyInjection).Assembly);

        return services;
    }
}
