using System.IdentityModel.Tokens.Jwt;
using MediatR;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.DTOs.Authentication;
using SeniorCrud.Application.Interfaces.Persistence;
using SeniorCrud.Application.Results;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.Application.Features.Authentication.Commands;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return Result<LoginResponseDto>.Unauthorized("Authentication.InvalidCredentials", "Email ou senha inválidos.");
        }

        var isValidPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash.Value);
        if (!isValidPassword)
        {
            return Result<LoginResponseDto>.Unauthorized("Authentication.InvalidCredentials", "Email ou senha inválidos.");
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Name);

        var expiresAtUtc = DateTimeOffset.UtcNow;
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        if (jwtToken.ValidTo != DateTime.MinValue)
        {
            expiresAtUtc = new DateTimeOffset(jwtToken.ValidTo, TimeSpan.Zero);
        }

        var response = new LoginResponseDto(user.Id, user.Name, accessToken, expiresAtUtc);

        return Result<LoginResponseDto>.Success(response);
    }
}
