using System.Linq.Expressions;
using AutoMapper;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Paging;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Enumerations;
using DistributedSystem.Contract.Services.V1.Product;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;

namespace Query.Application.UseCases.V1.Queries.Product;

public sealed class GetProductsQueryHandler : IQueryHandler<DistributedSystem.Contract.Services.V1.Product.Query.GetProductsQuery,
        PagedResult<Response.ProductResponse>>
{
    private readonly IMongoRepository<ProductProjection> _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IMongoRepository<ProductProjection> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<Response.ProductResponse>>> Handle(DistributedSystem.Contract.Services.V1.Product.Query.GetProductsQuery request, CancellationToken cancellationToken)
    {
        // products - IMongoQueryable<ProductProjection>
        var products = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? _productRepository.AsQueryable()
            : _productRepository.AsQueryable(
                x => x.Name.Contains(request.SearchTerm) || x.Description.Contains(request.SearchTerm));

        // Check pageIndex & pageSize - contraints
        var paging = Paging.Create(request.PageIndex, request.PageSize);

        int totalCount = await products.CountAsync();
        products = products.Skip((paging.PageIndex - 1) * paging.PageSize).Take(paging.PageSize);

        // Handle Sort One Column or Multiple Columns
        if (request.SortColumnAndOrder.Any())
        {
            foreach (var item in request.SortColumnAndOrder)
            {
                products = item.Value == SortOrder.Descending
                    ? products.OrderByDescending(GetSortProperty(item.Key))
                    : products.OrderBy(GetSortProperty(item.Key));
            }
        }
        else
        {
            products = request.SortOrder == SortOrder.Descending
                ? products.OrderByDescending(GetSortProperty(request))
                : products.OrderBy(GetSortProperty(request));
        }

        // Convert IMongoQueryable<ProductProjection> to List<ProductProjection>
        // Result more using MediatR
        var result = new List<Response.ProductResponse>();
        var productsList = await products.ToListAsync();

        foreach (var item in productsList)
        {
            result.Add(new Response.ProductResponse(item.DocumentId, item.Name, item.Price, item.Description));
        }

        // Củ chuối
        //var result = _mapper.Map<List<Response.ProductResponse>>(products);

        // Create PagedResult
        var pagedResult = PagedResult<Response.ProductResponse>.Create(
            items: result,
            pageIndex: paging.PageIndex,
            pageSize: paging.PageSize,
            totalCount: totalCount);

        return Result.Success(pagedResult);
    }

    private static Expression<Func<ProductProjection, object>> GetSortProperty(DistributedSystem.Contract.Services.V1.Product.Query.GetProductsQuery request)
        => request.SortColumn?.ToLower() switch
        {
            "name" => product => product.Name,
            "price" => product => product.Price,
            "description" => product => product.Description,
            _ => product => product.Id
        };

    private static Expression<Func<ProductProjection, object>> GetSortProperty(string propertyName)
        => propertyName.ToLower() switch
        {
            "name" => product => product.Name,
            "price" => product => product.Price,
            "description" => product => product.Description,
            _ => product => product.Id
        };
}
