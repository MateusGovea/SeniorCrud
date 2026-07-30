using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Addresses.Queries;

public sealed class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, Result<IReadOnlyList<AddressResponseDto>>>
{
    private readonly ICacheService _cacheService;
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    public GetAddressesQueryHandler(
        ICacheService cacheService,
        IAddressRepository addressRepository,
        IMapper mapper)
    {
        _cacheService = cacheService;
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<AddressResponseDto>>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var version = _cacheService.Get<string>(ApplicationCacheKeys.AddressesListVersion);
        if (string.IsNullOrWhiteSpace(version))
        {
            version = Guid.NewGuid().ToString("N");
            _cacheService.Set(ApplicationCacheKeys.AddressesListVersion, version, ApplicationCacheDurations.Long);
        }

        var cacheKey = ApplicationCacheKeys.AddressesList(version, pageNumber, pageSize, request.Search);
        var cachedResponse = _cacheService.Get<IReadOnlyList<AddressResponseDto>>(cacheKey);
        if (cachedResponse is not null)
        {
            return Result<IReadOnlyList<AddressResponseDto>>.Success(cachedResponse);
        }

        var addresses = await _addressRepository.GetPagedAsync(pageNumber, pageSize, request.Search, cancellationToken);
        var response = _mapper.Map<IReadOnlyList<AddressResponseDto>>(addresses);

        _cacheService.Set(cacheKey, response, ApplicationCacheDurations.Short);

        return Result<IReadOnlyList<AddressResponseDto>>.Success(response);
    }
}