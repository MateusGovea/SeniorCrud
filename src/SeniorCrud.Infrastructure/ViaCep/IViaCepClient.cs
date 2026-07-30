namespace SeniorCrud.Infrastructure.ViaCep;

public interface IViaCepClient
{
    Task<ViaCepAddressResponse?> GetAddressByCepAsync(string cep, CancellationToken cancellationToken = default);
}
