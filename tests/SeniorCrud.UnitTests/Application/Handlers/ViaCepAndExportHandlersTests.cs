using AutoMapper;
using FluentAssertions;
using Moq;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Abstractions.Csv;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Application.DTOs.Export;
using SeniorCrud.Application.Features.Export.Commands;
using SeniorCrud.Application.Features.ViaCep.Queries;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Domain.Entities;
using SeniorCrud.UnitTests.Common;

namespace SeniorCrud.UnitTests.Application.Handlers;

public class ViaCepAndExportHandlersTests
{
    private readonly IMapper _mapper = TestDataFactory.CreateMapper();

    [Fact]
    public async Task GetAddressByCepQueryHandler_Should_Return_NotFound_When_Cep_Invalid()
    {
        var handler = new GetAddressByCepQueryHandler(Mock.Of<ICacheService>(), Mock.Of<IViaCepClient>(), _mapper);

        var result = await handler.Handle(new GetAddressByCepQuery("123"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(SeniorCrud.Application.Results.ErrorType.NotFound);
    }

    [Fact]
    public async Task GetAddressByCepQueryHandler_Should_Return_Success_When_Client_Returns_Address()
    {
        var cache = new Mock<ICacheService>();
        cache.Setup(c => c.Get<SeniorCrud.Application.DTOs.ViaCep.ViaCepResponseDto>(It.IsAny<string>())).Returns((SeniorCrud.Application.DTOs.ViaCep.ViaCepResponseDto?)null);

        var viaCepClient = new Mock<IViaCepClient>();
        viaCepClient
            .Setup(c => c.GetAddressByCepAsync("01001000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ViaCepAddressResponse
            {
                Cep = "01001000",
                Logradouro = "Praca da Se",
                Bairro = "Se",
                Localidade = "Sao Paulo",
                Uf = "SP",
                Erro = false
            });

        var handler = new GetAddressByCepQueryHandler(cache.Object, viaCepClient.Object, _mapper);

        var result = await handler.Handle(new GetAddressByCepQuery("01001000"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Cep.Should().Be("01001000");
        cache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<SeniorCrud.Application.DTOs.ViaCep.ViaCepResponseDto>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task ExportUsersCsvCommandHandler_Should_Export_Filtered_Users()
    {
        var first = TestDataFactory.CreateUser(name: "Ana", email: "ana@test.com", cpf: "39053344705");
        var second = TestDataFactory.CreateUser(name: "Bruno", email: "bruno@test.com", cpf: "52998224725");

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.Query()).Returns(new List<User> { first, second }.AsQueryable());

        var csvService = new Mock<ICsvExportService>();
        csvService
            .Setup(service => service.Export(It.IsAny<IEnumerable<ExportUsersCsvDto>>()))
            .Returns("Id,Nome\n");

        var handler = new ExportUsersCsvCommandHandler(repository.Object, csvService.Object, _mapper);

        var result = await handler.Handle(new ExportUsersCsvCommand([first.Id]), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain("Nome");
    }
}
