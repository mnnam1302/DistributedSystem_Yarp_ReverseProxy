using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Query.Presentation.Abstractions;

namespace Query.Presentation.APIs
{
    public class ProductApi : ApiEndpoint, ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            const string BaseUrl = "/api/v{version:apiVersion}/products";

            var group1 = app.NewVersionedApi("products")
                .MapGroup(BaseUrl).HasApiVersion(1); //.RequireAuthorization();

            #region ========= Version 1 =========

            group1.MapGet(string.Empty, GetProductsV1);
            group1.MapGet("{productId}", GetProductsByIdV1);

            #endregion ========= Version 1 =========

            #region ========= Version 2 =========

            //var group2 = app.NewVersionedApi("products")
            //    .MapGroup(BaseUrl).HasApiVersion(2);

            //group1.MapGet(string.Empty, () => "");
            //group1.MapGet("{productId}", () => "");

            #endregion ========= Version 2 =========
        }

        #region ========= Version 1 =========

        public static async Task<IResult> GetProductsV1(ISender sender)
        {
            var result = await sender.Send(new DistributedSystem.Contract.Services.V1.Product.Query.GetProductsQuery());

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }

        public static async Task<IResult> GetProductsByIdV1(ISender sender, Guid productId)
        {
            var result = await sender.Send(new DistributedSystem.Contract.Services.V1.Product.Query.GetProductByIdQuery(productId));

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }

        #endregion ========= Version 1 =========
    }
}