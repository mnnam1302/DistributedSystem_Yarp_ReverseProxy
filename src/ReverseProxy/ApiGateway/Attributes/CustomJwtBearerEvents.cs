using ApiGateway.Abstractions;
using DistributedSystem.Contract.Services.V1.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway.Attributes
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly ICacheService _cacheService;

        public CustomJwtBearerEvents(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            // Token pass validate in Server JwtExtensions => Ok
            // Need to check this token in Redis???

            if (context.SecurityToken is JwtSecurityToken accessToken)
            {
                var requestToken = accessToken.RawData.ToString();

                var emailKey = accessToken.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;

                var authenticated = await _cacheService.GetAsync<Response.Authenticated>(emailKey);

                if (authenticated is null || authenticated.AccessToken != requestToken)
                {
                    context.HttpContext.Response.Headers.Add("IS-TOKEN-REVOKED", "true");
                    context.Fail("Authentication fail. Token has been revoked!");
                }
            }
            else
            {
                context.Fail("Authentication failed.");
            }
        }
    }
}
