using System.Security.Claims;

namespace Authorization.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims, string privateKey);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string publicKey);

    //JwtSecurityToken ValidateToken(string token);
}
