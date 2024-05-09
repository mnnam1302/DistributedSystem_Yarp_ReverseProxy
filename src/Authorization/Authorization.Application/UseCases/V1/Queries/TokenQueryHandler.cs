using Authorization.Application.Abstractions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;

namespace Authorization.Application.UseCases.V1.Queries;

public class TokenQueryHandler : IQueryHandler<Query.TokenQuery, Response.Authenticated>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;

    public TokenQueryHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
    }

    public async Task<Result<Response.Authenticated>> Handle(Query.TokenQuery request, CancellationToken cancellationToken)
    {
        //var accessToken = request.AccessToken;
        //var refreshToken = request.RefreshToken;

        //var principals = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
        //var emailKey = principals.FindFirstValue(ClaimTypes.Email).ToString();

        //var authenticated = await _cacheService.GetAsync<Response.Authenticated>(emailKey);

        //if (authenticated is null || authenticated.RefreshToken != refreshToken || authenticated.RefreshTokenExpiryTime <= DateTime.Now)
        //    throw new IdentityException.TokenException("Request token invalid!");

        //var newAccessToken = _jwtTokenService.GenerateAccessToken(principals.Claims);
        //var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        //var newAuthenticated = new Response.Authenticated
        //{
        //    AccessToken = newAccessToken,
        //    RefreshToken = newRefreshToken,
        //    RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
        //};

        //await _cacheService.SetAsync(emailKey, newAuthenticated, cancellationToken);

        //return Result.Success(newAuthenticated);

        return Result.Success(new Response.Authenticated());
    }
}
