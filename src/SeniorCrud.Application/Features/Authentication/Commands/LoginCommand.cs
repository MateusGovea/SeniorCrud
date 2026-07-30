using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Authentication;

namespace SeniorCrud.Application.Features.Authentication.Commands;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponseDto>;
