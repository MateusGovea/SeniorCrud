using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Infrastructure.ViaCep.Options;

namespace SeniorCrud.Infrastructure.ViaCep;

public sealed class ViaCepClient : IViaCepClient
{
    private readonly HttpClient _httpClient;
    private readonly ViaCepOptions _options;

    public ViaCepClient(HttpClient httpClient, IOptions<ViaCepOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<ViaCepAddressResponse?> GetAddressByCepAsync(string cep, CancellationToken cancellationToken = default)
    {
        var normalizedCep = NormalizeCep(cep);
        if (normalizedCep.Length != 8)
        {
            return null;
        }

        return await _httpClient.GetFromJsonAsync<ViaCepAddressResponse>($"ws/{normalizedCep}/json/", cancellationToken);
    }

    private static string NormalizeCep(string cep)
    {
        return new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
    }
}
