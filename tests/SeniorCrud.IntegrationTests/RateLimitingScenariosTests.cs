using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SeniorCrud.Application.DTOs.Authentication;
using SeniorCrud.IntegrationTests.Common;

namespace SeniorCrud.IntegrationTests;

public class RateLimitingScenariosTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Global_Policy_Allows_Requests_Below_Limit()
    {
        await using var factory = new SeniorCrudApiFactory()
            .WithWebHostBuilder(builder => builder.UseSetting("RateLimiting:Global:PermitLimit", "3"));
        using var client = await CreateAuthenticatedClientAsync(factory);

        for (var i = 0; i < 3; i++)
        {
            var response = await client.GetAsync("/api/v1/users");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task Global_Policy_Returns_429_With_RetryAfter_Above_Limit()
    {
        await using var factory = new SeniorCrudApiFactory()
            .WithWebHostBuilder(builder => builder.UseSetting("RateLimiting:Global:PermitLimit", "3"));
        using var client = await CreateAuthenticatedClientAsync(factory);

        for (var i = 0; i < 3; i++)
        {
            var allowed = await client.GetAsync("/api/v1/users");
            allowed.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var rejected = await client.GetAsync("/api/v1/users");
        rejected.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);

        rejected.Headers.RetryAfter.Should().NotBeNull();
        rejected.Headers.RetryAfter!.Delta.Should().NotBeNull();
        rejected.Headers.RetryAfter!.Delta!.Value.TotalSeconds.Should().BeGreaterThan(0);

        var payload = await DeserializeAsync<ApiResult>(rejected);
        payload.IsSuccess.Should().BeFalse();
        payload.Error.Code.Should().Be("RateLimit.Exceeded");
    }

    [Fact]
    public async Task Login_Policy_Is_More_Restrictive_Than_Global()
    {
        await using var factory = new SeniorCrudApiFactory()
            .WithWebHostBuilder(builder => builder.UseSetting("RateLimiting:Login:PermitLimit", "2"));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        for (var i = 0; i < 2; i++)
        {
            var login = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto("admin@test.com", "admin123"));
            login.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var rejected = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto("admin@test.com", "admin123"));
        rejected.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        rejected.Headers.RetryAfter.Should().NotBeNull();

        var payload = await DeserializeAsync<ApiResult>(rejected);
        payload.IsSuccess.Should().BeFalse();
        payload.Error.Code.Should().Be("RateLimit.Exceeded");
    }

    [Fact]
    public async Task Login_Policy_Does_Not_Consume_Global_Budget()
    {
        await using var factory = new SeniorCrudApiFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("RateLimiting:Login:PermitLimit", "1");
                builder.UseSetting("RateLimiting:Global:PermitLimit", "1");
            });
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto("admin@test.com", "admin123"));
        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginPayload = await DeserializeAsync<ApiResult<LoginResponseDto>>(login);

        var rejectedLogin = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto("admin@test.com", "admin123"));
        rejectedLogin.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginPayload.Value!.AccessToken);

        var users = await client.GetAsync("/api/v1/users");
        users.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health_Endpoints_Are_Not_Rate_Limited()
    {
        await using var factory = new SeniorCrudApiFactory()
            .WithWebHostBuilder(builder => builder.UseSetting("RateLimiting:Global:PermitLimit", "2"));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        for (var i = 0; i < 6; i++)
        {
            var health = await client.GetAsync("/health");
            health.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        for (var i = 0; i < 6; i++)
        {
            var liveness = await client.GetAsync("/liveness");
            liveness.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto("admin@test.com", "admin123"));
        var loginPayload = await DeserializeAsync<ApiResult<LoginResponseDto>>(loginResponse);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginPayload.Value!.AccessToken);

        return client;
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(json, JsonOptions);
        return result!;
    }
}
