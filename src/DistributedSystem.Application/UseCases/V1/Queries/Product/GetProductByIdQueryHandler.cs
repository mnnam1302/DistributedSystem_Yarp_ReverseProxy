using AutoMapper;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Domain.Abstractions.Repositories;
using DistributedSystem.Domain.Exceptions;
using DistributedSystem.Persistence;

namespace DistributedSystem.Application.UseCases.V1.Queries.Product
{
    public class GetProductByIdQueryHandler : IQueryHandler<Query.GetProductByIdQuery, Response.ProductResponse>
    {
        //private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepositoryBase;

        // Sử dụng trong TH nhiều DbContext => Flexible hơn
        private readonly IRepositoryBaseDbContext<ApplicationDbContext, Domain.Entities.Product, Guid> _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IRepositoryBaseDbContext<ApplicationDbContext, Domain.Entities.Product, Guid> productRepository,
            //IRepositoryBase<Domain.Entities.Product, Guid> productRepositoryBase,
            IMapper mapper)
        {
            _productRepository = productRepository;
            //_productRepositoryBase = productRepositoryBase;
            _mapper = mapper;
        }

        public async Task<Result<Response.ProductResponse>> Handle(Query.GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id)
                ?? throw new ProductException.ProductNotFoundException(request.Id);

            //var productBase = await _productRepositoryBase.FindByIdAsync(request.Id)
            //    ?? throw new ProductException.ProductNotFoundException(request.Id);

            var result = _mapper.Map<Response.ProductResponse>(product);

            return Result.Success(result);
        }
    }
}