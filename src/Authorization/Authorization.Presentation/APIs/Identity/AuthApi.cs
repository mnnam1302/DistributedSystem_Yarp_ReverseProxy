using Authorization.Presentation.Abstractions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Authorization.Presentation.APIs.Identity;

public class AuthApi : ApiEndpoint, ICarterModule
{
    private const string BaseUrl = "/api/v{version:apiVersion}/auth";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group1 = app.NewVersionedApi("authentication")
            .MapGroup(BaseUrl).HasApiVersion(1);

        group1.MapPost("login", AuthenticationV1);
        group1.MapPost("logout", LogoutV1);

        //var group2 = app.NewVersionedApi("authentication")
        //    .MapGroup(BaseUrl).HasApiVersion(2);

        // group2.MapGet("sign-in", () => "");
        // group2.MapGet("sign-out", () => "");
    }

    private static async Task<IResult> AuthenticationV1(ISender sender, [FromBody] DistributedSystem.Contract.Services.V1.Identity.Query.GetLoginQuery login)
    {
        var result = await sender.Send(login);

        if (result.IsFailure)
        {
            return HandlerFailure(result);
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> LogoutV1(ISender sender)
    {
        // Todo CODE
        return Results.Ok();
    }
}
