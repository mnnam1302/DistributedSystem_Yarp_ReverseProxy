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
        var authValue = await _cacheService.GetAsync<RedisKeyValue.AuthenticatedValue>(request.Email, cancellationToken)
            ?? throw new IdentityException.TokenException("Can not get value from Redis");

        var principle = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken, authValue.PublicKey);
        var emailKey = principle.FindFirstValue(ClaimTypes.Email).ToString();

        await _cacheService.RemoveAsync(emailKey, cancellationToken);

        return Result.Success();
    }
}
