using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Common.Caching;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Application.Features.Users.Commands;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<Result<UserResponseDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserResponseDto>.NotFound("Users.NotFound", "User was not found.");
        }

        var email = new Email(request.Email);
        if (await _userRepository.ExistsByEmailAsync(email, request.UserId, cancellationToken))
        {
            return Result<UserResponseDto>.Conflict("Users.EmailAlreadyInUse", "The informed e-mail is already in use.");
        }

        Cpf? cpf = null;
        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            cpf = new Cpf(request.Cpf);
            if (await _userRepository.ExistsByCpfAsync(cpf, request.UserId, cancellationToken))
            {
                return Result<UserResponseDto>.Conflict("Users.CpfAlreadyInUse", "The informed CPF is already in use.");
            }
        }

        var now = DateTimeOffset.UtcNow;
        var role = Enum.Parse<UserRole>(request.Role, ignoreCase: true);

        user.UpdateProfile(request.Nome, email, cpf, request.BirthDate, now);
        user.ChangeRole(role, now);

        if (request.IsActive)
        {
            user.Activate(now);
        }
        else
        {
            user.Deactivate(now);
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cacheService.Remove(ApplicationCacheKeys.User(user.Id));
        _cacheService.Remove(ApplicationCacheKeys.UsersListVersion);
        _cacheService.Remove(ApplicationCacheKeys.UserAddresses(user.Id));

        return Result<UserResponseDto>.Success(_mapper.Map<UserResponseDto>(user));
    }
}
