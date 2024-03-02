using Carter;
using DistributedSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using CommandV1 = DistributedSystem.Contract.Services.V1.Product;
//using CommandV2 = DistributedSystem.Contract.Services.V2.Product;

namespace DistributedSystem.Presentation.APIs.Products
{
    public class ProductApi : ApiEndpoint, ICarterModule
    {
        private const string BaseUrl = "/api/v{version:apiVersion}/products";

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            #region ========= Version 1 =========

            var group1 = app.NewVersionedApi("products")
                .MapGroup(BaseUrl).HasApiVersion(1).RequireAuthorization();

            group1.MapPost(string.Empty, CreateProductsV1);
            group1.MapGet(string.Empty, GetProductsV1);
            group1.MapGet("{productId}", GetProductsByIdV1);
            group1.MapDelete("{productId}", DeleteProductsV1);
            group1.MapPut("{productId}", UpdateProductsV1);

            #endregion ========= Version 1 =========


            #region ========= Version 2 =========

            // Tạm thời comment - FIX BUG sau, chưa ăn Version 02, nhưng check thì có, chắc do Swagger
            // => Đã fix xong
            //var group2 = app.NewVersionedApi("products")
            //    .MapGroup(BaseUrl).HasApiVersion(2);

            //group2.MapPost(string.Empty, () => "");
            //group2.MapGet(string.Empty, () => "");
            //group2.MapGet("{productId}", () => "");
            //group2.MapDelete("{productId}", () => "");
            //group2.MapPut("{productId}", () => "");

            #endregion ========= Version 2 =========
        }

        #region ========= Version 1 =========

        public static async Task<IResult> CreateProductsV1(ISender sender, [FromBody] CommandV1.Command.CreatedProductCommand createProduct)
        {
            var result = await sender.Send(createProduct);

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }

        public static async Task<IResult> GetProductsV1(ISender sender)
        {
            var result = await sender.Send(new CommandV1.Query.GetProductsQuery());
            return Results.Ok(result);
        }

        public static async Task<IResult> GetProductsByIdV1(ISender sender, Guid productId)
        {
            var result = await sender.Send(new CommandV1.Query.GetProductByIdQuery(productId));
            return Results.Ok(result);
        }

        public static async Task<IResult> DeleteProductsV1(ISender sender, Guid productId)
        {
            var result = await sender.Send(new CommandV1.Command.DeleteProductCommand(productId));
            return Results.Ok(result);
        }

        public static async Task<IResult> UpdateProductsV1(ISender sender, Guid productId, [FromBody] CommandV1.Command.UpdateProductCommand updateProduct)
        {
            var updateProductCommand = new CommandV1.Command.UpdateProductCommand(productId, updateProduct.Name, updateProduct.Price, updateProduct.Description);

            var result = await sender.Send(updateProductCommand);
            return Results.Ok(result);
        }

        #endregion ========= Version 1 =========


        #region ========= Version 2 =========

        #endregion ========= Version 2 =========
    }
}