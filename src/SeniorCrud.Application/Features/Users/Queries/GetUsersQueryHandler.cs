using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Users.Queries;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserListItemDto>>>
{
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(ICacheService cacheService, IUserRepository userRepository, IMapper mapper)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var version = _cacheService.Get<string>(ApplicationCacheKeys.UsersListVersion);
        if (string.IsNullOrWhiteSpace(version))
        {
            version = Guid.NewGuid().ToString("N");
            _cacheService.Set(ApplicationCacheKeys.UsersListVersion, version, ApplicationCacheDurations.Long);
        }

        var cacheKey = ApplicationCacheKeys.UsersList(version, pageNumber, pageSize, request.Search);
        var cachedResponse = _cacheService.Get<IReadOnlyList<UserListItemDto>>(cacheKey);
        if (cachedResponse is not null)
        {
            return Result<IReadOnlyList<UserListItemDto>>.Success(cachedResponse);
        }

        var users = await _userRepository.GetPagedAsync(pageNumber, pageSize, request.Search, cancellationToken);
        var response = _mapper.Map<IReadOnlyList<UserListItemDto>>(users);

        _cacheService.Set(cacheKey, response, ApplicationCacheDurations.Short);

        return Result<IReadOnlyList<UserListItemDto>>.Success(response);
    }
}
