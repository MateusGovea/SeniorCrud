using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SeniorCrud.Api.HealthChecks;

public sealed class MemoryCacheHealthCheck : IHealthCheck
{
    private const string ProbeKey = "healthcheck:memorycache";
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheHealthCheck(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        _memoryCache.Set(ProbeKey, "ok", TimeSpan.FromMinutes(1));
        var isHealthy = _memoryCache.TryGetValue(ProbeKey, out var value) && value is string cacheValue && cacheValue == "ok";

        return Task.FromResult(isHealthy
            ? HealthCheckResult.Healthy("Memory cache is healthy.")
            : HealthCheckResult.Unhealthy("Memory cache is unavailable."));
    }
}
