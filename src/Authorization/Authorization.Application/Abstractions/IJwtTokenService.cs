using System.Security.Claims;

namespace Authorization.Application.Abstractions
{
    public interface IJwtTokenService
    {
        string GenerateToken(List<Claim> claims);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}