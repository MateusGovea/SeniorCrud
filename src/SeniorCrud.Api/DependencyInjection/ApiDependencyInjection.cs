using FluentValidation;
using Microsoft.AspNetCore.ResponseCompression;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SeniorCrud.Api.ExceptionHandling;
using SeniorCrud.Api.Extensions;
using SeniorCrud.Api.HealthChecks;
using SeniorCrud.Application.Common.Diagnostics;
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
        services.AddOpenApiVersioning();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHealthChecks()
            .AddCheck<SqlServerHealthCheck>("sqlserver", tags: ["ready"])
            .AddCheck<ViaCepHealthCheck>("viacep", tags: ["ready"])
            .AddCheck<MemoryCacheHealthCheck>("memory_cache", tags: ["live", "ready"]);
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddOptions();

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

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(configuration["OpenTelemetry:ServiceName"] ?? "SeniorCrud.Api"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(ApplicationActivitySource.SourceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request_content_length", request.ContentLength ?? 0);
                            activity.SetTag("http.user_agent", request.Headers.UserAgent.ToString());
                            activity.SetTag("correlation.id", request.Headers["X-Correlation-ID"].ToString());
                        };
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddConsoleExporter();
            });

        services.AddValidatorsFromAssembly(typeof(ApiDependencyInjection).Assembly);

        return services;
    }
}
