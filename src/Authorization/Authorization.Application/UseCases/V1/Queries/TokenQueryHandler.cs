using System.Security.Claims;
using Authorization.Application.Abstractions;
using Authorization.Domain.Exceptions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;

namespace Authorization.Application.UseCases.V1.Queries;

public class TokenQueryHandler : IQueryHandler<Query.TokenQuery, Response.Authenticated>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;
    private readonly IRSAKeyGenerator _rsaKeyGenerator;

    public TokenQueryHandler(IJwtTokenService jwtTokenService, ICacheService cacheService, IRSAKeyGenerator rsaKeyGenerator)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
        _rsaKeyGenerator = rsaKeyGenerator;
    }

    public async Task<Result<Response.Authenticated>> Handle(Query.TokenQuery request, CancellationToken cancellationToken)
    {
        /*
         * Nhận 3 thông tin: Email, AccessToken, RefreshToken
         * Email: kiểm tra tồn tài không bằng cách Get Value Cache với Key là Email
         * Access token m gửi tào lao => Tạo ra th mới là đi
         * Refresh token xem có khớp với cache không
         */

        var accessToken = request.AccessToken;
        var refreshToken = request.RefreshToken;

        // Step 01: Get Cache Value from Redis - Email
        var authValue = await _cacheService.GetAsync<RedisKeyValue.AuthenticatedValue>(request.Email, cancellationToken)
            ?? throw new IdentityException.TokenException("Can not get value from Redis");

        // Step 02: Get Principal from expired token - Access Token
        var principals = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken, authValue.PublicKey);
        var emailKey = principals.FindFirstValue(ClaimTypes.Email).ToString();

        // Step 03: Check Refresh Token
        if (authValue.RefreshToken != refreshToken || authValue.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new IdentityException.TokenException("Request token invalid!");
        }

        // Step 04: Generate new key pair
        var rsaKeys = _rsaKeyGenerator.GenerateRsaKeyPair();

        // Step 05: Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(principals.Claims, rsaKeys.privateKey);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var newAuthenticated = new Response.Authenticated
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
        };

        // Step 06: Set AuthenticatedValue to Redis - Key Value
        var newAuthenticatedValue = new RedisKeyValue.AuthenticatedValue
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = newAuthenticated.RefreshTokenExpiryTime,
            PublicKey = rsaKeys.publicKey,
        };

        await _cacheService.SetAsync(emailKey, newAuthenticatedValue, cancellationToken);

        return Result.Success(newAuthenticated);
    }
}
