using SeniorCrud.Application.Common.Messaging;

namespace SeniorCrud.Application.Features.Users.Commands;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;
