using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using SeniorCrud.Infrastructure.ViaCep;
using SeniorCrud.Infrastructure.ViaCep.Options;

namespace SeniorCrud.UnitTests.Infrastructure;

public class ViaCepClientTests
{
    [Fact]
    public async Task GetAddressByCepAsync_Should_Return_Null_When_Cep_Invalid()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = new HttpClient(handler);
        var client = new ViaCepClient(httpClient, Options.Create(new ViaCepOptions()));

        var result = await client.GetAddressByCepAsync("123", CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAddressByCepAsync_Should_Call_Correct_Endpoint_And_Parse_Response()
    {
        HttpRequestMessage? capturedRequest = null;
        var responseJson = "{\"cep\":\"01001-000\",\"logradouro\":\"Praca da Se\",\"erro\":false}";

        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler);
        var client = new ViaCepClient(httpClient, Options.Create(new ViaCepOptions { BaseUrl = "https://viacep.com.br/", TimeoutSeconds = 2 }));

        var result = await client.GetAddressByCepAsync("01001-000", CancellationToken.None);

        result.Should().NotBeNull();
        result!.Cep.Should().Be("01001-000");
        capturedRequest.Should().NotBeNull();
        capturedRequest!.RequestUri!.ToString().Should().Contain("ws/01001000/json/");
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }
}
