using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Domain.Abstractions.Repositories;
using DistributedSystem.Domain.Exceptions;

namespace DistributedSystem.Application.UseCases.V1.Commands.Product;

public class DeleteProductCommandHandler : ICommandHandler<Command.DeleteProductCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRespository;

    public DeleteProductCommandHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRespository)
    {
        _productRespository = productRespository;
    }

    public async Task<Result> Handle(Command.DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRespository.FindByIdAsync(request.Id)
            ?? throw new ProductException.ProductNotFoundException(request.Id);

        product.Delete();
        _productRespository.Remove(product);

        return Result.Success();
    }
}
