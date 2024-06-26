﻿using AutoMapper;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;
using Query.Domain.Exceptions;

namespace Query.Application.UseCases.V1.Queries.Product;

public sealed class GetProductByIdHandler : IQueryHandler<DistributedSystem.Contract.Services.V1.Product.Query.GetProductByIdQuery,
               Response.ProductResponse>
{
    private readonly IMongoRepository<ProductProjection> _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(IMongoRepository<ProductProjection> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<Response.ProductResponse>> Handle(DistributedSystem.Contract.Services.V1.Product.Query.GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindOneAsync(p => p.DocumentId == request.Id)
           ?? throw new ProductException.ProductNotFoundException(request.Id);

        //var productBase = await _productRepositoryBase.FindByIdAsync(request.Id)
        //    ?? throw new ProductException.ProductNotFoundException(request.Id);

        var result = new Response.ProductResponse(product.DocumentId, product.Name, product.Price, product.Description);

        // Củ chuối
        //var result = _mapper.Map<Response.ProductResponse>(product);
        return Result.Success(result);
    }
}
