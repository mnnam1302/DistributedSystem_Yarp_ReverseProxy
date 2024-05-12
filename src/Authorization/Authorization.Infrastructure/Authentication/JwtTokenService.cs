using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Authorization.Application.Abstractions;
using Authorization.Infrastructure.DependecyInjection.Options;
using Authorization.Infrastructure.Encryption;
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

    public string GenerateAccessToken(IEnumerable<Claim> claims, string privateKey)
    {
        var privatekey = RsaKeyConverterService.CreateRsaFromPrivateKey(privateKey);
        var signatureCredentials = new SigningCredentials(new RsaSecurityKey(privatekey), SecurityAlgorithms.RsaSha256);

        /*
         * Old code using one same screte key
         * Khá là giống với cái mình cấu hình ở JwtExtensions.cs
         * var screteKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
         * var signatureCredentials = new SigningCredentials(screteKey, SecurityAlgorithms.HmacSha256)
         */

        var tokenOptions = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtOptions.ExpireMin), // 2 mins
            signingCredentials: signatureCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[128];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
            var refreshToken = Convert.ToBase64String(bytes);
            return refreshToken;
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string publicKey)
    {
        /*
         * Old code using one same secret key
         * var Key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey)
         */

        var publickey = RsaKeyConverterService.CreateRsaFromPublicKey(publicKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // Set to true if you want to validate the Audience
            ValidateIssuer = false, // Set to true if you want to validate the Issuer
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(Key),
            IssuerSigningKey = new RsaSecurityKey(publickey),
            ClockSkew = TimeSpan.Zero // No clock skew for simplicity (adjust as needed)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCulture))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    //public JwtSecurityToken ValidateToken(string token)
    //{
    //    if (token is null)
    //    {
    //        return null;
    //    }

    //    try
    //    {
    //        var tokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateAudience = false,
    //            ValidateIssuer = false,
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new RsaSecurityKey(publicKey),
    //            ClockSkew = TimeSpan.Zero
    //        };

    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
    //        var jwtSecurityToken = securityToken as JwtSecurityToken;
    //        return jwtSecurityToken;
    //    }
    //    catch
    //    {
    //        return null;
    //    }
    //}
}
