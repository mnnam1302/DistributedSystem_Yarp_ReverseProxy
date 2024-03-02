using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Domain.Abstractions.Repositories;

namespace DistributedSystem.Application.UseCases.V1.Commands.Product
{
    public sealed class CreateProductCommandHandler : ICommandHandler<Command.CreatedProductCommand>
    {
        private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;

        public CreateProductCommandHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result> Handle(Command.CreatedProductCommand request, CancellationToken cancellationToken)
        {
            // Thay vì class thì ở đây mình dùng record
            // Cách triển khai IRequest và IRequestHandler rất hay

            // New memeber has to call Create method to create new instance product
            // Inside bussiness logic, New member won't miss, ignore business logic

            // Trong này mình Raise Event
            var product = Domain.Entities.Product.Create(Guid.NewGuid(), request.Name, request.Price, request.Description);

            _productRepository.Add(product);

            return Result.Success();
        }
    }
}