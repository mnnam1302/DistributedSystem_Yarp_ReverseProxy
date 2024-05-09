using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DistributedSystem.Application.Abstractions;
using DistributedSystem.Infrastructure.DependencyInjection.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DistributedSystem.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions jwtOptions = new JwtOptions();

    public JwtTokenService(IConfiguration configuration)
    {
        configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        // Khá là giống với cái mình cấu hình ở JwtExtensions.cs
        var screteKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
        var signatureCredentials = new SigningCredentials(screteKey, SecurityAlgorithms.HmacSha256);

        /**
         * Chú ý: Lúc này mình setup cho Extension là ValidateIssuer = false, ValidateAudience = false
         * => 2 thằng này không giống nhau nhưng chung Secrete key thì nó vẫn ăn
         * Còn khi đặt nó là ValidateIssuer = true, ValidateAudience = true => th Generator và th Validate là 2 th tách biệt nhau luôn => Cố gắng hiểu
         * => JwtExtensions là th server, là th Validate
         * => JwtTokenService hay GenerateAccessToken là th Generator
         *
         * Bất cứ chuỗi token nào mình Generate ra: Có thể Gen ra từ một hệ thống khác - có thể code bằng Java,... nhưng nó có chung những cái cấu hình này và screte key thì khi lên Postman request thì th Server JwtExtensions nó Validate thấy à có Issuer, Audience, có ValidateIssuer như này là nó giải mã ra
         * => Chứ không phải 2 th này là 1
         * => Th có thể code bằng ngôn ngữ khác, ở một nơi khác
         */
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
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var Key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        // Cấu hình như th Server Valdate ở JwtExtensions.cs
        // Đầu tiên, phải kiểm tra token Expire này có đúng token mà mình đã cấp phát hay không - đúng cái secret key hay không?
        // Lỡ ngta fake rồi sao?
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateLifetime = false, //here we are saying that we don't care about the token's expiration date
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
