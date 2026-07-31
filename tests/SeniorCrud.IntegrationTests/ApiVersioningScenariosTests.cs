using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.DTOs.Authentication;
using SeniorCrud.Infrastructure.Authentication;
using SeniorCrud.IntegrationTests.Common;

namespace SeniorCrud.IntegrationTests;

public class ApiVersioningScenariosTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Get_Users_Should_Return_Success_On_Current_Version()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync("/api/v1/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Users_Without_Version_Should_Return_NotFound()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_Users_On_Unsupported_Version_Should_Return_NotFound()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync("/api/v9/users");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Swagger_V1_Document_Should_Be_Available()
    {
        await using var factory = new SeniorCrudApiFactory()
            .WithWebHostBuilder(builder => builder
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.RemoveAll<IPasswordHasher>();
                    services.AddSingleton<IPasswordHasher, PasswordHasher>();
                }));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        var document = await response.Content.ReadAsStringAsync();
        document.Should().Contain("\"/api/v1/users\"");
        document.Should().Contain("\"/api/v1/auth/login\"");
        document.Should().Contain("securitySchemes");
        document.Should().Contain("\"bearer\"");
    }

    [Fact]
    public async Task Responses_Should_Report_Supported_Versions_Header()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto("admin@test.com", "admin123"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("api-supported-versions");
        response.Headers.GetValues("api-supported-versions").Should().Contain(value => value.Contains("1.0"));
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(SeniorCrudApiFactory factory)
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
        return JsonSerializer.Deserialize<T>(json, JsonOptions)!;
    }
}
