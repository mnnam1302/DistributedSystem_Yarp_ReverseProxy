using DistributedSystem.Application.Abstractions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;
using DistributedSystem.Domain.Exceptions;
using System.Security.Claims;
using static DistributedSystem.Domain.Exceptions.IdentityException;

namespace DistributedSystem.Application.UseCases.V1.Queries.Identity
{
    public class GetRefreshTokenQueryHandler : IQueryHandler<Query.TokenQuery, Response.Authenticated>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICacheService _cacheService;

        public GetRefreshTokenQueryHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
        {
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
        }

        public async Task<Result<Response.Authenticated>> Handle(Query.TokenQuery request, CancellationToken cancellationToken)
        {
            var accessToken = request.AccessToken;
            var refreshToken = request.RefreshToken;

            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
            var emailKey = principal.FindFirstValue(ClaimTypes.Email).ToString();

            var authenticated = await _cacheService.GetAsync<Response.Authenticated>(emailKey);

            // Xem cơ chế mình muốn throw ra một Exception nào đó và bắt nó như thế nào
            if (authenticated is null || authenticated.RefreshToken != refreshToken || authenticated.RefreshTokenExpiryTime <= DateTime.Now)
                throw new TokenException("Request token invalid!");

            var newAccessToken = _jwtTokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            var newAuthenticated = new Response.Authenticated
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5) // Chỗ này tùy mình có muốn set lại hay không
            };

            // Redis automatically overrive value based on key
            // No need to GetAsync and then UpdateAsync
            await _cacheService.SetAsync(emailKey, newAuthenticated, cancellationToken);

            return Result.Success(newAuthenticated);
        }
    }
}