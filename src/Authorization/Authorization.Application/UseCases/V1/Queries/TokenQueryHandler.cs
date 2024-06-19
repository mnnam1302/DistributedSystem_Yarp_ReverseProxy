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
            1. Get cache by email from Redis
            2. Validate token user's request => verify token
            3. Compare email from token with email from request
            4. Check refresh token
            5. Generate new token && Create result
            6. Set AuthenticatedValue to Redis - Key Value
         */

        // 1.
        var authValue = await _cacheService.GetAsync<RedisKeyValue.AuthenticatedValue>(request.Email, cancellationToken)
            ?? throw new IdentityException.TokenException("Can not get value from Redis");

        // 2.
        var principals = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken, authValue.PublicKey);
        var emailKey = principals.FindFirstValue(ClaimTypes.Email).ToString();

        // 3.
        if (emailKey != request.Email)
        {
            throw new IdentityException.TokenException("Email from token is not match with email from request");
        }

        // 4.
        if (authValue.RefreshToken != request.RefreshToken || authValue.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new IdentityException.TokenException("Request token invalid!");
        }

        // 5.
        var rsaKeys = _rsaKeyGenerator.GenerateRsaKeyPair();

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
