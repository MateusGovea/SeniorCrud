using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using SeniorCrud.Api.Configurations;
using SeniorCrud.Api.Middleware;
using System.Threading.RateLimiting;

namespace SeniorCrud.Api.Extensions;

/// <summary>
/// Registers the official ASP.NET Core rate limiting pipeline using a global
/// Fixed Window policy per client IP, with a stricter budget for the login route.
/// </summary>
public static class RateLimitingExtensions
{
    public static IServiceCollection AddApplicationRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>() ?? new RateLimitingOptions();
        options.EnsureValid();

        services.AddRateLimiter(rateLimiter =>
        {
            rateLimiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiter.OnRejected = RateLimitingRejectionHandler.HandleAsync;

            rateLimiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var clientId = GetClientIdentifier(httpContext);

                if (IsLoginRoute(httpContext))
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"{RateLimitingPolicies.Login}:{clientId}",
                        factory: _ => options.Login.Create());
                }

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: clientId,
                    factory: _ => options.Global.Create());
            });
        });

        return services;
    }

    public static WebApplication UseApplicationRateLimiting(this WebApplication app)
    {
        app.UseRateLimiter();
        return app;
    }

    private static bool IsLoginRoute(HttpContext httpContext)
    {
        var path = httpContext.Request.Path.Value;
        return path is not null && path.Contains("/auth/login", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetClientIdentifier(HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var firstEntry = forwardedFor.Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(firstEntry))
            {
                return firstEntry;
            }
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
