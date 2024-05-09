using Authorization.Application.Abstractions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;

namespace Authorization.Application.UseCases.V1.Commands;

public class RevokeTokenCommandHandler : ICommandHandler<Command.RevokeTokenCommand>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;

    public RevokeTokenCommandHandler(IJwtTokenService jwtTokenService, ICacheService cachService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cachService;
    }

    public async Task<Result> Handle(Command.RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        //string AccessToken = request.AccessToken;

        //var principal = _jwtTokenService.GetPrincipalFromExpiredToken(AccessToken);
        //var emailKey = principal.FindFirstValue(ClaimTypes.Email).ToString();

        //var authenticated = await _cacheService.GetAsync<Response.Authenticated>(emailKey);

        //if (authenticated is null)
        //    throw new Exception("Can not get value from Redis");

        //await _cacheService.RemoveAsync(emailKey, cancellationToken);

        return Result.Success();
    }
}
