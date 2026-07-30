using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace SeniorCrud.Infrastructure.Authentication;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var rawId = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? Principal?.FindFirstValue(ClaimTypes.Sid)
                        ?? Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(rawId, out var parsed) ? parsed : null;
        }
    }

    public string? Nome => Principal?.FindFirstValue(ClaimTypes.Name);

    public IReadOnlyCollection<Claim> Claims => Principal?.Claims.ToArray() ?? [];

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;
}
