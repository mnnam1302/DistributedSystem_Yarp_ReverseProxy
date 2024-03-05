﻿using Authorization.Presentation.Abstractions;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Authorization.Presentation.APIs.Identity
{
    public class AuthApi : ApiEndpoint, ICarterModule
    {
        private const string BaseUrl = "/api/v{version:apiVersion}/auth";

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group1 = app.NewVersionedApi("authentication")
                .MapGroup(BaseUrl).HasApiVersion(1);

            group1.MapPost("login", AuthenticationV1);

            var group2 = app.NewVersionedApi("authentication")
                .MapGroup(BaseUrl).HasApiVersion(2);

            group2.MapPost("login", () => "");
        }

        private static IResult AuthenticationV1()
        {
            return Results.Ok();
        }
    }
}