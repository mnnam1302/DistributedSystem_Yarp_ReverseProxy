using Carter;
using DistributedSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSystem.Presentation.APIs.Identity
{
    public class TokenApi : ApiEndpoint, ICarterModule
    {
        private const string BaseUrl = "/api/v{version:apiVersion}/token";
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group1 = app.NewVersionedApi("token")
                .MapGroup(BaseUrl).HasApiVersion(1).RequireAuthorization();

            /**
             * Lý do tại sao ở đây mình RequireAuthorized nó luôn
             * 
             * => Nếu AccessToken sai => Nó không vô được API endpoint luôn
             * => Không bao giờ nhảy vô RefreshV1
             * => AccessToken phải CHUẨN mới nhảy vào RefreshV1 được
             */

            group1.MapPost("refresh", RefreshV1);
            group1.MapPost("revoke", RevokeV1);
        }

        public static async Task<IResult> RefreshV1(ISender sender, HttpContext httpContext, [FromBody] Contract.Services.V1.Identity.Query.TokenQuery token)
        {
            // Get access token from HttpContext, no need to get from request body
            // Access token inside Body, user can enter anything => Not care about it
            var AccessToken = await httpContext.GetTokenAsync("access_token");

            var result = await sender.Send(new Contract.Services.V1.Identity.Query.TokenQuery(AccessToken, token.RefreshToken));

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }

        public static async Task<IResult> RevokeV1(ISender sender, HttpContext httpContext)
        {
            var AccessToken = await httpContext.GetTokenAsync("access_token");

            var result = await sender.Send(new Contract.Services.V1.Identity.Command.RevokeTokenCommand(AccessToken));

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }
    }
}
