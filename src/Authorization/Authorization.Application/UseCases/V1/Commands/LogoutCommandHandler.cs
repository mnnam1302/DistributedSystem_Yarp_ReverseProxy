using System.Security.Claims;
using Authorization.Application.Abstractions;
using Authorization.Domain.Exceptions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;

namespace Authorization.Application.UseCases.V1.Commands;

public class LogoutCommandHandler : ICommandHandler<Command.LogoutCommand>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;

    public LogoutCommandHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(Command.LogoutCommand request, CancellationToken cancellationToken)
    {
        /*
            1. Get Key-Value by email from Redis
            2. Validate token user's request => verify token
            3. Compare email from token with email from request
            4. Remove key-value from Redis
         */

        // 1.
        var authValue = await _cacheService.GetAsync<RedisKeyValue.AuthenticatedValue>(request.Email, cancellationToken)
            ?? throw new IdentityException.TokenException("Can not get value from Redis");

        // 2.
        var principle = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken, authValue.PublicKey);
        var emailKey = principle.FindFirstValue(ClaimTypes.Email).ToString();

        // 3.
        if (emailKey != request.Email)
        {
            throw new IdentityException.TokenException("Email from token is not match with email from request");
        }

        // 4.
        await _cacheService.RemoveAsync(emailKey, cancellationToken);

        return Result.Success();
    }
}
