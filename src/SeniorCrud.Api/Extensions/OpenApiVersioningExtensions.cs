using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using SeniorCrud.Api.Configurations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SeniorCrud.Api.Extensions;

/// <summary>
/// Centralizes API versioning + OpenAPI/Swagger registration and pipeline wiring,
/// keeping Program.cs lean. Swagger documents are generated per API version.
/// </summary>
public static class OpenApiVersioningExtensions
{
    public static IServiceCollection AddOpenApiVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddSwaggerGen();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        return services;
    }

    public static WebApplication UseOpenApiWithVersioning(this WebApplication app)
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"{app.Environment.EnvironmentName} - {description.GroupName.ToUpperInvariant()}");
            }
        });

        return app;
    }
}
