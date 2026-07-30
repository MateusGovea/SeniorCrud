using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.ViaCep;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.ViaCep.Queries;

public sealed class GetAddressByCepQueryHandler : IRequestHandler<GetAddressByCepQuery, Result<ViaCepResponseDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IViaCepClient _viaCepClient;
    private readonly IMapper _mapper;

    public GetAddressByCepQueryHandler(ICacheService cacheService, IViaCepClient viaCepClient, IMapper mapper)
    {
        _cacheService = cacheService;
        _viaCepClient = viaCepClient;
        _mapper = mapper;
    }

    public async Task<Result<ViaCepResponseDto>> Handle(GetAddressByCepQuery request, CancellationToken cancellationToken)
    {
        if (!IsValidCep(request.Cep))
        {
            return Result<ViaCepResponseDto>.NotFound("ViaCep.NotFound", "CEP was not found.");
        }

        var cacheKey = ApplicationCacheKeys.ViaCep(request.Cep);
        var cachedResponse = _cacheService.Get<ViaCepResponseDto>(cacheKey);
        if (cachedResponse is not null)
        {
            return Result<ViaCepResponseDto>.Success(cachedResponse);
        }

        var response = await _viaCepClient.GetAddressByCepAsync(request.Cep, cancellationToken);
        if (response is null || response.Erro == true || string.IsNullOrWhiteSpace(response.Cep))
        {
            return Result<ViaCepResponseDto>.NotFound("ViaCep.NotFound", "CEP was not found.");
        }

        var mappedResponse = _mapper.Map<ViaCepResponseDto>(response);
        _cacheService.Set(cacheKey, mappedResponse, ApplicationCacheDurations.Long);

        return Result<ViaCepResponseDto>.Success(mappedResponse);
    }

    private static bool IsValidCep(string cep)
    {
        var digits = new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
        return digits.Length == 8;
    }
}
