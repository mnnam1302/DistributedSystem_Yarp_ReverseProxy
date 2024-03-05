using System.Security.Claims;

namespace Authorization.Application.Abstractions
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(List<Claim> claims);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}