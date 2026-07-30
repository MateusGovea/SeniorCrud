using SeniorCrud.Application.Common.Messaging;
using SeniorCrud.Application.DTOs.Users;

namespace SeniorCrud.Application.Features.Users.Queries;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 20, string? Search = null)
    : IQuery<IReadOnlyList<UserListItemDto>>;
