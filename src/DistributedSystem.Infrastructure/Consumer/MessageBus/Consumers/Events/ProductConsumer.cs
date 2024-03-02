using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Infrastructure.Consumer.Abstractions.Messages;
using DistributedSystem.Infrastructure.Consumer.Abstractions.Repositories;
using DistributedSystem.Infrastructure.Consumer.Models;
using MediatR;

namespace DistributedSystem.Infrastructure.Consumer.MessageBus.Consumers.Events
{
    public static class ProductConsumer
    {
        public class ProductCreatedConsumer : Consumer<DomainEvent.ProductCreated>
        {
            public ProductCreatedConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository) : base(sender, eventRepository)
            {
            }
        }

        public class ProductUpdatedConsumer : Consumer<DomainEvent.ProductUpdated>
        {
            public ProductUpdatedConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository) : base(sender, eventRepository)
            {
            }
        }

        public class ProductDeletedConsumer : Consumer<DomainEvent.ProductDeleted>
        {
            public ProductDeletedConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository) : base(sender, eventRepository)
            {
            }
        }
    }
}