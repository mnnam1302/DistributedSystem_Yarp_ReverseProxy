using AutoMapper;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace DistributedSystem.Application.UseCases.V1.Queries.Product
{
    public class GetProductsQueryHandler : IQueryHandler<Query.GetProductsQuery, List<Response.ProductResponse>>
    {
        private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository, 
            IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<Response.ProductResponse>>> Handle(Query.GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.FindAll().ToListAsync();

            var result = _mapper.Map<List<Response.ProductResponse>>(products);

            return Result.Success(result);
        }
    }
}