using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;

namespace SeniorCrud.Application.Features.Users.Commands;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IAddressRepository _addressRepository;
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(ICacheService cacheService, IUserRepository userRepository, IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.NotFound("Users.NotFound", "User was not found.");
        }

        var now = DateTimeOffset.UtcNow;
        var addresses = user.Addresses.ToList();

        foreach (var address in addresses)
        {
            user.RemoveAddress(address.Id, now);
            _addressRepository.Remove(address);
        }

        _userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cacheService.Remove(ApplicationCacheKeys.User(user.Id));
        _cacheService.Remove(ApplicationCacheKeys.UsersListVersion);
        _cacheService.Remove(ApplicationCacheKeys.UserAddresses(user.Id));

        foreach (var address in addresses)
        {
            _cacheService.Remove(ApplicationCacheKeys.Address(address.Id));
        }

        if (addresses.Count > 0)
        {
            _cacheService.Remove(ApplicationCacheKeys.AddressesListVersion);
        }

        return Result.Success();
    }
}
