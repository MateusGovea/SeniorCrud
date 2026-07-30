using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.Addresses;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Addresses.Queries;

public sealed class GetAddressesByUserQueryHandler : IRequestHandler<GetAddressesByUserQuery, Result<IReadOnlyList<AddressResponseDto>>>
{
    private readonly ICacheService _cacheService;
    private readonly IAddressRepository _addressRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAddressesByUserQueryHandler(
        ICacheService cacheService,
        IAddressRepository addressRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _cacheService = cacheService;
        _addressRepository = addressRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<AddressResponseDto>>> Handle(GetAddressesByUserQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = ApplicationCacheKeys.UserAddresses(request.UserId);
        var cachedResponse = _cacheService.Get<IReadOnlyList<AddressResponseDto>>(cacheKey);
        if (cachedResponse is not null)
        {
            return Result<IReadOnlyList<AddressResponseDto>>.Success(cachedResponse);
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<IReadOnlyList<AddressResponseDto>>.NotFound("Users.NotFound", "User was not found.");
        }

        var addresses = await _addressRepository.ListByUserIdAsync(request.UserId, cancellationToken);
        var response = _mapper.Map<IReadOnlyList<AddressResponseDto>>(addresses);

        _cacheService.Set(cacheKey, response, ApplicationCacheDurations.Short);

        return Result<IReadOnlyList<AddressResponseDto>>.Success(response);
    }
}
