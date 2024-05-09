using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Authorization.Application.Abstractions;
using Authorization.Infrastructure.DependecyInjection.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions jwtOptions = new JwtOptions();

    public JwtTokenService(IConfiguration configuration)
    {
        configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims, RSAParameters privatKey)
    {
        // Mã hóa bất đối xứng
        var rsaKey = new RsaSecurityKey(privatKey);
        var signatureCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

        // Khá là giống với cái mình cấu hình ở JwtExtensions.cs
        //var screteKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
        //var signatureCredentials = new SigningCredentials(screteKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtOptions.ExpireMin),
            signingCredentials: signatureCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, RSAParameters publicKey)
    {
        //var Key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(Key),
            IssuerSigningKey = new RsaSecurityKey(publicKey),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        //if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
        //    throw new SecurityTokenException("Invalid token");

        return principal;
    }
}