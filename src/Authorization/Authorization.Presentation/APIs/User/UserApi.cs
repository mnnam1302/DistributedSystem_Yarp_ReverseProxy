using Authorization.Presentation.Abstractions;
using Carter;
using DistributedSystem.Contract.Services.V1.Identity;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Authorization.Presentation.APIs.User
{
    public class UserApi : ApiEndpoint, ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            const string BaseUrl = "/api/v{version:apiVersion}/user";

            var group1 = app.NewVersionedApi("user")
                .MapGroup(BaseUrl).HasApiVersion(1);

            group1.MapPost("register", RegisterV1);
        }

        private static async Task<IResult> RegisterV1(ISender sender, [FromBody] Command.RegisterCommand register)
        {
            var result = await sender.Send(register);

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }
    }
}