using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Users.Queries;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponseDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(ICacheService cacheService, IUserRepository userRepository, IMapper mapper)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = ApplicationCacheKeys.User(request.UserId);
        var cachedResponse = _cacheService.Get<UserResponseDto>(cacheKey);
        if (cachedResponse is not null)
        {
            return Result<UserResponseDto>.Success(cachedResponse);
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserResponseDto>.NotFound("Users.NotFound", "User was not found.");
        }

        var response = _mapper.Map<UserResponseDto>(user);
        _cacheService.Set(cacheKey, response, ApplicationCacheDurations.Medium);

        return Result<UserResponseDto>.Success(response);
    }
}
