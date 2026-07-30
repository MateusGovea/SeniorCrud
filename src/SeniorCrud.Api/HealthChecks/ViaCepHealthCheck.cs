using Microsoft.Extensions.Diagnostics.HealthChecks;
using SeniorCrud.Application.Abstractions.ViaCep;

namespace SeniorCrud.Api.HealthChecks;

public sealed class ViaCepHealthCheck : IHealthCheck
{
    private const string KnownHealthyCep = "01001000";
    private readonly IViaCepClient _viaCepClient;

    public ViaCepHealthCheck(IViaCepClient viaCepClient)
    {
        _viaCepClient = viaCepClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var response = await _viaCepClient.GetAddressByCepAsync(KnownHealthyCep, cancellationToken);

        if (response is null || response.Erro == true)
        {
            return HealthCheckResult.Unhealthy("ViaCEP endpoint is unavailable.");
        }

        return HealthCheckResult.Healthy("ViaCEP endpoint is healthy.");
    }
}
