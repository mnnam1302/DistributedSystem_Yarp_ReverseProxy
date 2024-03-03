using MediatR;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;
using Query.Infrastructure.Abstractions;

namespace Query.Infrastructure.Consumer
{
    public static class ProductConsumer
    {
        public class ProductCreatedConsumer : Consumer<DistributedSystem.Contract.Services.V1.Product.DomainEvent.ProductCreated>
        {
            public ProductCreatedConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository)
                : base(sender, eventRepository)
            {
            }
        }

        public class ProductUpdatedConsumer : Consumer<DistributedSystem.Contract.Services.V1.Product.DomainEvent.ProductUpdated>
        {
            public ProductUpdatedConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository)
                : base(sender, eventRepository)
            {
            }
        }

        public class ProductDeletedConsumer : Consumer<DistributedSystem.Contract.Services.V1.Product.DomainEvent.ProductDeleted>
        {
            public ProductDeletedConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository)
                : base(sender, eventRepository)
            {
            }
        }
    }
}