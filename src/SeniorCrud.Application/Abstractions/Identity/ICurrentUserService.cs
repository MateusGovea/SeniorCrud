using System.Security.Claims;

namespace SeniorCrud.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Nome { get; }

    IReadOnlyCollection<Claim> Claims { get; }
}
