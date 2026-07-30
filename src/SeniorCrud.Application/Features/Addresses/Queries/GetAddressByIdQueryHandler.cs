using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Addresses.Queries;

public sealed class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Result<AddressResponseDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    public GetAddressByIdQueryHandler(ICacheService cacheService, IAddressRepository addressRepository, IMapper mapper)
    {
        _cacheService = cacheService;
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<Result<AddressResponseDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = ApplicationCacheKeys.Address(request.AddressId);
        var cachedResponse = _cacheService.Get<AddressResponseDto>(cacheKey);
        if (cachedResponse is not null)
        {
            return Result<AddressResponseDto>.Success(cachedResponse);
        }

        var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);
        if (address is null)
        {
            return Result<AddressResponseDto>.NotFound("Addresses.NotFound", "Address was not found.");
        }

        var response = _mapper.Map<AddressResponseDto>(address);
        _cacheService.Set(cacheKey, response, ApplicationCacheDurations.Medium);

        return Result<AddressResponseDto>.Success(response);
    }
}
