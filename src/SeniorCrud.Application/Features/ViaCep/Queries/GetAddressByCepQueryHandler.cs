using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Application.DTOs.ViaCep;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.ViaCep.Queries;

public sealed class GetAddressByCepQueryHandler : IRequestHandler<GetAddressByCepQuery, Result<ViaCepResponseDto>>
{
    private readonly IViaCepClient _viaCepClient;
    private readonly IMapper _mapper;

    public GetAddressByCepQueryHandler(IViaCepClient viaCepClient, IMapper mapper)
    {
        _viaCepClient = viaCepClient;
        _mapper = mapper;
    }

    public async Task<Result<ViaCepResponseDto>> Handle(GetAddressByCepQuery request, CancellationToken cancellationToken)
    {
        if (!IsValidCep(request.Cep))
        {
            return Result<ViaCepResponseDto>.NotFound("ViaCep.NotFound", "CEP was not found.");
        }

        var response = await _viaCepClient.GetAddressByCepAsync(request.Cep, cancellationToken);
        if (response is null || response.Erro == true || string.IsNullOrWhiteSpace(response.Cep))
        {
            return Result<ViaCepResponseDto>.NotFound("ViaCep.NotFound", "CEP was not found.");
        }

        return Result<ViaCepResponseDto>.Success(_mapper.Map<ViaCepResponseDto>(response));
    }

    private static bool IsValidCep(string cep)
    {
        var digits = new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
        return digits.Length == 8;
    }
}
