using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Paging;
using DistributedSystem.Contract.Enumerations;

namespace DistributedSystem.Contract.Services.V1.Product;

public static class Query
{
    //public record GetProductsQuery(string? SearchTerm, int PageIndex, int PageSize)
    //    : IQuery<List<Response.ProductResponse>>
    //{
    //}

    //public record GetProductsQuery(string? SearchTerm, int PageIndex, int PageSize)
    //: IQuery<PagedResult<Response.ProductResponse>>
    //{
    //}

    public record GetProductsQuery(string? SearchTerm, string? SortColumn, string? SortOrder,
        IDictionary<string, SortOrder>? SortColumnAndOrder,
        int PageIndex, int PageSize) : IQuery<PagedResult<Response.ProductResponse>>;

    public record GetProductByIdQuery(Guid Id) : IQuery<Response.ProductResponse>;
}
