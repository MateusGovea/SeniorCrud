using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using SeniorCrud.Infrastructure.Caching;

namespace SeniorCrud.UnitTests.Infrastructure;

public class MemoryCacheServiceTests
{
    [Fact]
    public void MemoryCacheService_Should_Set_Get_And_Remove()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(memoryCache);

        service.Set("key", "value", TimeSpan.FromMinutes(1));
        var cached = service.Get<string>("key");

        cached.Should().Be("value");

        service.Remove("key");

        service.Get<string>("key").Should().BeNull();
    }
}
