﻿using Authorization.Application.Abstractions;
using Authorization.Domain.Abstractions.Repositories;
using Authorization.Domain.Entities;
using Authorization.Domain.Exceptions;
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
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IRepositoryBase<AppUser, Guid> _userRepository;

        public GetLoginQueryHandler(
            IJwtTokenService jwtTokenService, 
            ICacheService cacheService,
            IPasswordHasherService passwordHasherService,
            IRepositoryBase<AppUser, Guid> userRepository
            )
        {
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
            _passwordHasherService = passwordHasherService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 1. Get user by email
        /// 2. Using salt to hash password to compare with password in database
        /// 3. Get user claims
        /// 4. Generate access token and refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result<Response.Authenticated>> Handle(Query.GetLoginQuery request, CancellationToken cancellationToken)
        {
            //Check user
            var user = await _userRepository.FindSingleAsync(x => x.Email == request.Email, cancellationToken);

            if (user is null)
                throw new AppUserException.UserNotFoundByEmailException(request.Email);

            var isAuthentication = _passwordHasherService.VerifyPassword(request.Password, 
                                                                            user.PasswordHash, 
                                                                            user.PasswordSalt);

            if (!isAuthentication)
                throw new IdentityException.AuthenticatedException();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email),
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