using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Users;

namespace SeniorCrud.Application.Features.Users.Commands;

public sealed record UpdateUserCommand(
    Guid UserId,
    string Nome,
    string Email,
    string? Cpf,
    DateOnly? BirthDate,
    bool IsActive) : ICommand<UserResponseDto>;
