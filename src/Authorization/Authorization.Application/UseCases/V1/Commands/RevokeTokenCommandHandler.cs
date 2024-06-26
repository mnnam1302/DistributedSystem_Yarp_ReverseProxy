﻿using System.Security.Claims;
using Authorization.Application.Abstractions;
using Authorization.Domain.Exceptions;
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
        /*
         * Nhận 2 thông tin: Email, AccessToken
         * Check email tồn tại bằng cách Get Value Cache với Key là Email
         * Access token m gửi tào lao => Tạo ra th mới là đi => GetPrincipalFromExpiredToken - xem nó đúng chuẩn lúc mình Create không?
         */
        var accessToken = request.AccessToken;

        var authValue = await _cacheService.GetAsync<RedisKeyValue.AuthenticatedValue>(request.Email, cancellationToken)
            ?? throw new IdentityException.TokenException("Can not get value from Redis");

        var principle = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken, authValue.PublicKey);
        var emailKey = principle.FindFirstValue(ClaimTypes.Email).ToString();

        await _cacheService.RemoveAsync(emailKey, cancellationToken);

        return Result.Success();
    }
}
