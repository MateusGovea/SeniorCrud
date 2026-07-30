using System.Security.Claims;

namespace SeniorCrud.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string nome, IEnumerable<Claim>? additionalClaims = null);
}
