using Authorization.Presentation.Abstractions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Authorization.Presentation.APIs.Identity;

public class TokenApi : ApiEndpoint, ICarterModule
{
    private const string BaseUrl = "/api/v{version:apiVersion}/token";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Please check. Are we need to RequireAuthorization() here ?
        var group1 = app.NewVersionedApi("token")
            .MapGroup(BaseUrl).HasApiVersion(1);

        //var group1 = app.NewVersionedApi("token")
        //    .MapGroup(BaseUrl).HasApiVersion(1).RequireAuthorization();

        group1.MapPost("refresh", RefreshTokenV1);
        group1.MapPost("revoke", RevokeTokenV1);
    }

    private static async Task<IResult> RefreshTokenV1(ISender sender, HttpContext httpContext, [FromBody] DistributedSystem.Contract.Services.V1.Identity.Query.TokenQuery token)
    {
        //var accessToken = await httpContext.GetTokenAsync("access_token");
        var result = await sender.Send(new DistributedSystem.Contract.Services.V1.Identity.Query.TokenQuery(token.Email, token.AccessToken, token.RefreshToken));

        if (result.IsFailure)
        {
            return HandlerFailure(result);
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> RevokeTokenV1(ISender sender, [FromBody] DistributedSystem.Contract.Services.V1.Identity.Command.RevokeTokenCommand accessToken)
    {
        var result = await sender.Send(accessToken);

        if (result.IsFailure)
        {
            return HandlerFailure(result);
        }

        return Results.Ok(result);
    }
}
