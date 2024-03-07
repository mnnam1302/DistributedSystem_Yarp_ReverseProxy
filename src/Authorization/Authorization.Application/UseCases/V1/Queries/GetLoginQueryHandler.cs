using Authorization.Application.Abstractions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;
using System.Security.Claims;

namespace Authorization.Application.UseCases.V1.Queries
{
    public class GetLoginQueryHandler : IQueryHandler<Query.GetLoginQuery, Response.Authenticated>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICacheService _cacheService;
        private readonly IHashPasswordService _hashPasswordService;

        public GetLoginQueryHandler(
            IJwtTokenService jwtTokenService, 
            ICacheService cacheService, 
            IHashPasswordService hashPasswordService)
        {
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
            _hashPasswordService = hashPasswordService;
        }

        public async Task<Result<Response.Authenticated>> Handle(Query.GetLoginQuery request, CancellationToken cancellationToken)
        {
            // Check Email and Password

            // Step 1: Get User by Email => Get salt && hash password in DB

            // test
            // Hash password use in case user register => Result: string hashPassword && salt
            // Assume, Register
            var hashPasswordDB = _hashPasswordService.HashPassword(request.Password, out string salt);

            // Step 2: Verify Password
            var isPasswordValid = _hashPasswordService.VerifyPassword(request.Password, hashPasswordDB, salt);

            // Get User's claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "Nhat Nam"),
                new(ClaimTypes.Email, request.Email),
                new(ClaimTypes.Role, "Junior .NET")
            };


            string accessToken = _jwtTokenService.GenerateAccessToken(claims);
            string refreshToken = _jwtTokenService.GenerateRefreshToken();

            var result = new Response.Authenticated
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
            };

            await _cacheService.SetAsync(request.Email, result, cancellationToken);

            return Result.Success(result);
        }
    }
}