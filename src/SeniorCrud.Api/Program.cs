using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SeniorCrud.Api.Middleware;
using SeniorCrud.Api.DependencyInjection;
using SeniorCrud.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApiFoundation(builder.Configuration);

var app = builder.Build();

// Apply pending migrations automatically on startup
await app.ApplyMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms (CorrelationId: {CorrelationId})";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
        diagnosticContext.Set("Endpoint", httpContext.GetEndpoint()?.DisplayName ?? "unknown");
    };
});
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});
app.MapHealthChecks("/readiness", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

app.Run();

public partial class Program;

internal static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IPersistenceSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var timeout = TimeSpan.FromSeconds(5);
        var maxRetries = 10;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await seeder.SeedAsync();
                logger.LogInformation("Database migrations applied successfully");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                logger.LogWarning(ex,
                    "Failed to apply migrations (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}...",
                    attempt, maxRetries, timeout);
                await Task.Delay(timeout);
            }
        }
    }
}
