namespace SeniorCrud.Infrastructure.ViaCep.Options;

public sealed class ViaCepOptions
{
    public const string SectionName = "ViaCep";

    public string BaseUrl { get; set; } = "https://viacep.com.br/";

    public int TimeoutSeconds { get; set; } = 2;
}
