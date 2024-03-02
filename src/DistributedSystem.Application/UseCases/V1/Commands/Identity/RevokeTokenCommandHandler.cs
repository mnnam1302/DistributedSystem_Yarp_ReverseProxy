using DistributedSystem.Application.Abstractions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;
using DistributedSystem.Domain.Exceptions;
using MediatR;
using System.Security.Claims;

namespace DistributedSystem.Application.UseCases.V1.Commands.Identity
{
    public class RevokeTokenCommandHandler : ICommandHandler<Command.RevokeTokenCommand>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICacheService _cacheService;

        public RevokeTokenCommandHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
        {
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
        }

        public async Task<Result> Handle(Command.RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            // When access pass here, it means the token still valid
            var AccessToken = request.AccessToken;
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(AccessToken);
            var emailKey = principal.FindFirstValue(ClaimTypes.Email).ToString();

            var authenticated = await _cacheService.GetAsync<Response.Authenticated>(emailKey);

            if (authenticated is null)
                throw new Exception("Can not get value from Redis");

            await _cacheService.RemoveAsync(emailKey, cancellationToken);

            return Result.Success();
        }
    }
}