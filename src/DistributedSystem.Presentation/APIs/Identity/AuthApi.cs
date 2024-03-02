using Carter;
using DistributedSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

//using CommandV1 = DistributedSystem.Contract.Services.V1.Identity;

namespace DistributedSystem.Presentation.APIs.Identity
{
    public class AuthApi : ApiEndpoint, ICarterModule
    {
        private const string BaseUrl = "/api/v{version:apiVersion}/auth";

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group1 = app.NewVersionedApi("authentication")
                .MapGroup(BaseUrl).HasApiVersion(1).RequireAuthorization();

            group1.MapPost("login", AuthenticationV1).AllowAnonymous();
        }

        public static async Task<IResult> AuthenticationV1(ISender sender, [FromBody] Contract.Services.V1.Identity.Query.GetLoginQuery login)
        {
            var result = await sender.Send(login);

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }
    }
}