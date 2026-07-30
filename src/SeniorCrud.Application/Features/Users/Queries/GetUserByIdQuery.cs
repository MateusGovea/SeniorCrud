using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Users;

namespace SeniorCrud.Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponseDto>;
