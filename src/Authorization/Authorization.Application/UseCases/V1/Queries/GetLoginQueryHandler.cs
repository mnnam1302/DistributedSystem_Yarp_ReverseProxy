using Authorization.Application.Abstractions;
using Authorization.Domain.Abstractions.Repositories;
using Authorization.Domain.Entities;
using Authorization.Domain.Exceptions;
using Authorization.Persistence;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;
using System.Security.Claims;

namespace Authorization.Application.UseCases.V1.Queries;

public class GetLoginQueryHandler : IQueryHandler<Query.GetLoginQuery, Response.Authenticated>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IRepositoryBase<AppUser, Guid> _userRepository;
    private readonly IRSAKeyGenerator _encryptService;

    public GetLoginQueryHandler(
        IJwtTokenService jwtTokenService,
        ICacheService cacheService,
        IPasswordHasherService passwordHasherService,
        IRepositoryBase<AppUser, Guid> userRepository,
        IRSAKeyGenerator encryptService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
        _passwordHasherService = passwordHasherService;
        _userRepository = userRepository;
        _encryptService = encryptService;
    }

    public async Task<Result<Response.Authenticated>> Handle(Query.GetLoginQuery request, CancellationToken cancellationToken)
    {
        /*
            1. Get User by Email
            2. Verify password
            3. Get user claims
            4. Generate key pair RSA
            5. Generate token && Create result
            6. Set AuthenticatedValue to Redis - Key Value
         */

        // 1.
        var user = await _userRepository.FindSingleAsync(x => x.Email == request.Email, cancellationToken)
            ?? throw new AppUserException.UserNotFoundByEmailException(request.Email);

        // 2.
        var isMatch = _passwordHasherService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isMatch)
        {
            throw new IdentityException.AuthenticatedException();
        }

        // 3.
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.FullName),
            new (ClaimTypes.Email, user.Email),
        };

        // 4.
        var rsaKeys = _encryptService.GenerateRsaKeyPair();

        // 5.
        string accessToken = _jwtTokenService.GenerateAccessToken(claims, rsaKeys.privateKey);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();

        var result = new Response.Authenticated
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
        };

        // 6.
        var authValue = new RedisKeyValue.AuthenticatedValue
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            RefreshTokenExpiryTime = result.RefreshTokenExpiryTime,
            PublicKey = rsaKeys.publicKey
        };

        await _cacheService.SetAsync(request.Email, authValue, cancellationToken);

        return Result.Success(result);
    }
}
