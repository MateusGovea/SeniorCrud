using System.Security.Claims;

namespace SeniorCrud.Infrastructure.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Nome { get; }

    IReadOnlyCollection<Claim> Claims { get; }
}
