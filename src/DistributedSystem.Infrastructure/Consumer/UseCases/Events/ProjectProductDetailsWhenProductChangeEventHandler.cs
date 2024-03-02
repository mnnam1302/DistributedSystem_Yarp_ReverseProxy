using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Domain.Exceptions;
using DistributedSystem.Infrastructure.Consumer.Abstractions.Repositories;
using DistributedSystem.Infrastructure.Consumer.Exceptions;
using DistributedSystem.Infrastructure.Consumer.Models;

namespace DistributedSystem.Infrastructure.Consumer.UseCases.Events
{
    // Kiểu Centralized cực kỳ hay
    internal class ProjectProductDetailsWhenProductChangeEventHandler
        : ICommandHandler<DomainEvent.ProductCreated>,
        ICommandHandler<DomainEvent.ProductUpdated>,
        ICommandHandler<DomainEvent.ProductDeleted>
    {
        // Repository working MongoDB
        private readonly IMongoRepository<ProductProjection> _productRepository;

        public ProjectProductDetailsWhenProductChangeEventHandler(IMongoRepository<ProductProjection> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result> Handle(DomainEvent.ProductCreated request, CancellationToken cancellationToken)
        {
            var product = new ProductProjection()
            {
                DocumentId = request.Id, // Carefully - Id bên Event là DocumentId của Product chứ không phải là thuộc tính Id
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
            };

            // Create a new Product
            await _productRepository.InsertOneAsync(product);

            return Result.Success();
        }

        public async Task<Result> Handle(DomainEvent.ProductUpdated request, CancellationToken cancellationToken)
        {
            // Find and update a Product
            // Throw ProductException.ProductNotFoundException(request.Id); này không GOOD
            // => Nhằm lẫn bên kia => Tạo ra một cái như bên Domain và throw nó ra bên ProductProjection cho chuẩn
            //var product = await _productRepository.FindByIdAsync(request.Id.ToString()) 
            //    ?? throw new ProductException.ProductNotFoundException(request.Id);
            var product = await _productRepository.FindOneAsync(x => x.DocumentId == request.Id)
                ?? throw new ConsumerProductException.ConsumerProductNotFoundException(request.Id);

            // Update Product
            product.Name = request.Name;
            product.Price = request.Price;
            product.Description = request.Description;
            product.ModifiedOnUtc = DateTimeOffset.UtcNow;

            await _productRepository.ReplaceOneAsync(product);

            return Result.Success();
        }

        public async Task<Result> Handle(DomainEvent.ProductDeleted request, CancellationToken cancellationToken)
        {
            // Find and delete a Product
            var product = await _productRepository.FindOneAsync(x => x.DocumentId == request.Id)
                ?? throw new ConsumerProductException.ConsumerProductNotFoundException(request.Id);

            // Xóa theo Id là SAIs
            //await _productRepository.DeleteByIdAsync(request.Id.ToString());

            await _productRepository.DeleteOneAsync(x => x.DocumentId == request.Id);


            return Result.Success();
        }
    }
}