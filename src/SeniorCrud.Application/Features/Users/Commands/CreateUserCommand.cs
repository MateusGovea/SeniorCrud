using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Users;

namespace SeniorCrud.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string Nome,
    string Email,
    string Password,
    string? Cpf,
    DateOnly? BirthDate,
    string Role = "User") : ICommand<UserResponseDto>;
