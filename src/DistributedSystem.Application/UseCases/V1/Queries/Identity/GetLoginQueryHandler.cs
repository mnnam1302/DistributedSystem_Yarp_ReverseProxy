using DistributedSystem.Application.Abstractions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;
using System.Security.Claims;

namespace DistributedSystem.Application.UseCases.V1.Queries.Identity
{
    public class GetLoginQueryHandler : IQueryHandler<Query.GetLoginQuery, Response.Authenticated>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICacheService _cacheService;

        public GetLoginQueryHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
        {
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
        }

        public async Task<Result<Response.Authenticated>> Handle(Query.GetLoginQuery request, CancellationToken cancellationToken)
        {
            // Check User  

            // Genarate JWT Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Role, "Senior .NET Lead")
            };

            string accessToken = _jwtTokenService.GenerateAccessToken(claims);
            string refreshToken = _jwtTokenService.GenerateRefreshToken();


            var result = new Response.Authenticated
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
            };

            // Caching result logging
            await _cacheService.SetAsync(request.Email, result);


            return Result.Success(result);
        }
    }
}