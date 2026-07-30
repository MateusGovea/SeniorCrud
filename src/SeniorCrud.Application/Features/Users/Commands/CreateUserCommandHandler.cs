using AutoMapper;
using MediatR;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.DTOs.Users;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;
using SeniorCrud.Domain.Entities;
using SeniorCrud.Domain.Enums;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Application.Features.Users.Commands;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<Result<UserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken: cancellationToken))
        {
            return Result<UserResponseDto>.Conflict("Users.EmailAlreadyInUse", "The informed e-mail is already in use.");
        }

        Cpf? cpf = null;
        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            cpf = new Cpf(request.Cpf);
            if (await _userRepository.ExistsByCpfAsync(cpf, cancellationToken: cancellationToken))
            {
                return Result<UserResponseDto>.Conflict("Users.CpfAlreadyInUse", "The informed CPF is already in use.");
            }
        }

        var passwordHash = new PasswordHash(_passwordHasher.HashPassword(request.Password));
        var now = DateTimeOffset.UtcNow;

        var user = new User(
            id: Guid.NewGuid(),
            name: request.Nome,
            email: email,
            passwordHash: passwordHash,
            role: UserRole.User,
            createdAtUtc: now,
            cpf: cpf,
            birthDate: request.BirthDate);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserResponseDto>.Success(_mapper.Map<UserResponseDto>(user));
    }
}
