using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.DTOs.Authentication;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.DTOs.ViaCep;
using SeniorCrud.IntegrationTests.Common;

namespace SeniorCrud.IntegrationTests;

public class ApiScenariosTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Login_Should_Return_Success_For_Valid_User()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var createResponse = await client.PostAsJsonAsync("/api/users", new CreateUserDto("Login User", "login@test.com", "password123", "39053344705", new DateOnly(1990, 1, 1)));
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto("login@test.com", "password123"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await DeserializeAsync<ApiResult<LoginResponseDto>>(response);

        payload.IsSuccess.Should().BeTrue();
        payload.Value.Should().NotBeNull();
        payload.Value!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Users_Should_Support_Crud_Flow()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        var create = await client.PostAsJsonAsync("/api/users", new CreateUserDto("CRUD User", "crud@test.com", "password123", "52998224725", null));
        var created = await DeserializeAsync<ApiResult<UserResponseDto>>(create);

        created.IsSuccess.Should().BeTrue();
        var userId = created.Value!.Id;

        var get = await client.GetAsync($"/api/users/{userId}");
        var getPayload = await DeserializeAsync<ApiResult<UserResponseDto>>(get);
        getPayload.IsSuccess.Should().BeTrue();

        var update = await client.PutAsJsonAsync($"/api/users/{userId}", new UpdateUserDto(Guid.Empty, "CRUD User Updated", "crud.updated@test.com", "52998224725", null, true));
        var updated = await DeserializeAsync<ApiResult<UserResponseDto>>(update);
        updated.IsSuccess.Should().BeTrue();
        updated.Value!.Nome.Should().Be("CRUD User Updated");

        var list = await client.GetAsync("/api/users?pageNumber=1&pageSize=10");
        var listed = await DeserializeAsync<ApiResult<IReadOnlyList<UserListItemDto>>>(list);
        listed.IsSuccess.Should().BeTrue();
        listed.Value.Should().Contain(item => item.Id == userId);

        var delete = await client.DeleteAsync($"/api/users/{userId}");
        var deleted = await DeserializeAsync<ApiResult>(delete);
        deleted.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Addresses_Should_Support_Crud_Flow()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        var userCreate = await client.PostAsJsonAsync("/api/users", new CreateUserDto("Address User", "address@test.com", "password123", "39053344705", null));
        var createdUser = await DeserializeAsync<ApiResult<UserResponseDto>>(userCreate);
        var userId = createdUser.Value!.Id;

        var createAddress = await client.PostAsJsonAsync("/api/addresses", new CreateAddressDto(userId, "01001000", "Rua Teste", "10", null, "Centro", "Sao Paulo", "SP", true));
        var createdAddress = await DeserializeAsync<ApiResult<AddressResponseDto>>(createAddress);
        createdAddress.IsSuccess.Should().BeTrue();

        var addressId = createdAddress.Value!.Id;

        var getById = await client.GetAsync($"/api/addresses/{addressId}");
        var got = await DeserializeAsync<ApiResult<AddressResponseDto>>(getById);
        got.IsSuccess.Should().BeTrue();

        var update = await client.PutAsJsonAsync($"/api/addresses/{addressId}", new UpdateAddressDto(Guid.Empty, "30140071", "Av Brasil", "55", "Apto 1", "Centro", "Belo Horizonte", "MG", false));
        var updated = await DeserializeAsync<ApiResult<AddressResponseDto>>(update);
        updated.IsSuccess.Should().BeTrue();
        updated.Value!.City.Should().Be("Belo Horizonte");

        var byUser = await client.GetAsync($"/api/users/{userId}/addresses");
        var listed = await DeserializeAsync<ApiResult<IReadOnlyList<AddressResponseDto>>>(byUser);
        listed.IsSuccess.Should().BeTrue();
        listed.Value.Should().Contain(address => address.Id == addressId);

        var delete = await client.DeleteAsync($"/api/addresses/{addressId}");
        var deleted = await DeserializeAsync<ApiResult>(delete);
        deleted.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ViaCep_Should_Return_Address_When_Cep_Is_Known()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        var response = await client.GetAsync("/api/viacep/01001000");
        var payload = await DeserializeAsync<ApiResult<ViaCepResponseDto>>(response);

        payload.IsSuccess.Should().BeTrue();
        payload.Value!.Uf.Should().Be("SP");
    }

    [Fact]
    public async Task ExportCsv_Should_Return_Csv_Content()
    {
        await using var factory = new SeniorCrudApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        await client.PostAsJsonAsync("/api/users", new CreateUserDto("Csv One", "csv1@test.com", "password123", "39053344705", null));
        await client.PostAsJsonAsync("/api/users", new CreateUserDto("Csv Two", "csv2@test.com", "password123", "52998224725", null));

        var response = await client.GetAsync("/api/users/export/csv");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/csv");

        var csv = await response.Content.ReadAsStringAsync();
        csv.Should().Contain("Nome");
        csv.Should().Contain("Csv One");
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(json, JsonOptions);
        return result!;
    }
}
