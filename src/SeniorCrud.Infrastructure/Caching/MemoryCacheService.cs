using Microsoft.Extensions.Caching.Memory;

namespace SeniorCrud.Infrastructure.Caching;

public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
    {
        _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}
