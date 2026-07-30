namespace SeniorCrud.Infrastructure.Caching;

public interface ICacheService
{
    T? Get<T>(string key);

    void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);

    void Remove(string key);
}
