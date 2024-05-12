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
        // Step 01: Get User by Email
        var user = await _userRepository.FindSingleAsync(x => x.Email == request.Email, cancellationToken)
            ?? throw new AppUserException.UserNotFoundByEmailException(request.Email);

        // Step 02: Verify password
        var isAuthentication = _passwordHasherService.VerifyPassword(request.Password, user.PasswordHash!, user.PasswordSalt);

        if (!isAuthentication)
        {
            throw new IdentityException.AuthenticatedException();
        }

        // Step 03: Get user claims
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.FullName),
            new (ClaimTypes.Email, user.Email),
        };

        // Step 04: Generate key pair RSA
        var rsaKeys = _encryptService.GenerateRsaKeyPair();

        // Step 05: Generate token && Create result
        string accessToken = _jwtTokenService.GenerateAccessToken(claims, rsaKeys.privateKey);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();

        var result = new Response.Authenticated
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
        };

        // Step 06: Set AuthenticatedValue to Redis - Key Value
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
