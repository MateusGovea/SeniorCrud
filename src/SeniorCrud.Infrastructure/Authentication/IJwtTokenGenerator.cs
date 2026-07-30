using System.Security.Claims;

namespace SeniorCrud.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string nome, IEnumerable<Claim>? additionalClaims = null);
}
