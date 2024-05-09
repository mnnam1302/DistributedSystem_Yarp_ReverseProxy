using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;
using Query.Domain.Exceptions;

namespace Query.Application.UseCases.V1.Commands.Product;

public class ProductProjectionDetailsWhenProductChangeEventHandler
    : ICommandHandler<DomainEvent.ProductCreated>,
    ICommandHandler<DomainEvent.ProductUpdated>,
    ICommandHandler<DomainEvent.ProductDeleted>
{
    private readonly IMongoRepository<ProductProjection> _productRepository;

    public ProductProjectionDetailsWhenProductChangeEventHandler(IMongoRepository<ProductProjection> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result> Handle(DomainEvent.ProductCreated request, CancellationToken cancellationToken)
    {
        var productProjection = new ProductProjection()
        {
            DocumentId = request.Id,
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
        };

        await _productRepository.InsertOneAsync(productProjection);

        return Result.Success();
    }

    public async Task<Result> Handle(DomainEvent.ProductUpdated request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindOneAsync(p => p.DocumentId == request.Id)
            ?? throw new ProductException.ProductNotFoundException(request.Id);

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ModifiedOnUtc = DateTime.UtcNow;

        await _productRepository.ReplaceOneAsync(product);

        return Result.Success();
    }

    public async Task<Result> Handle(DomainEvent.ProductDeleted request, CancellationToken cancellationToken)
    {
        var productProjection = await _productRepository.FindOneAsync(p => p.DocumentId == request.Id)
            ?? throw new ProductException.ProductNotFoundException(request.Id);

        await _productRepository.DeleteOneAsync(p => p.DocumentId == request.Id);

        return Result.Success();
    }
}
