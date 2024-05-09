using System.Security.Claims;
using System.Security.Cryptography;

namespace Authorization.Application.Abstractions
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims, RSAParameters privateKey);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, RSAParameters publicKey);
    }
}