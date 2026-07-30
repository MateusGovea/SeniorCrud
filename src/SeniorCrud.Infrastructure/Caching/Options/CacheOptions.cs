namespace SeniorCrud.Infrastructure.Caching.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public int CepAbsoluteExpirationHours { get; set; } = 24;

    public int CepNegativeCacheMinutes { get; set; } = 5;
}
