using Authorization.Presentation.Abstractions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Authorization.Presentation.APIs.User;

public class UserApi : ApiEndpoint, ICarterModule
{
    private const string BaseUrl = "/api/v{version:apiVersion}/users";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group1 = app.NewVersionedApi("users")
            .MapGroup(BaseUrl).HasApiVersion(1);

        group1.MapPost("", CreateUsersV1);
        group1.MapPut("", UpdateUsersV1);
        group1.MapDelete("", DeleteUsersV1);

    }

    #region ================= Version 01 =====================

    private static async Task<IResult> CreateUsersV1(ISender Sender, [FromBody] DistributedSystem.Contract.Services.V1.Identity.Command.RegisterUserCommand registerUserCommand)
    {
        var result = await Sender.Send(registerUserCommand);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateUsersV1(ISender Sender)
    {
        // TODO CODE

        return Results.Ok();
    }

    private static async Task<IResult> DeleteUsersV1(ISender Sender)
    {
        // TODO CODE

        return Results.Ok();
    }

    #endregion ================= Version 01 =====================
}
