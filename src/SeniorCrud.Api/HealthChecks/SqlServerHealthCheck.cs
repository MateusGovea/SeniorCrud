using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SeniorCrud.Persistence.Contexts;

namespace SeniorCrud.Api.HealthChecks;

public sealed class SqlServerHealthCheck : IHealthCheck
{
    private readonly SeniorCrudDbContext _dbContext;

    public SqlServerHealthCheck(SeniorCrudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
        return canConnect
            ? HealthCheckResult.Healthy("SQL Server connection is healthy.")
            : HealthCheckResult.Unhealthy("SQL Server connection is unavailable.");
    }
}
