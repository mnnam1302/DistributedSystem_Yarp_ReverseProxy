using DistributedSystem.Infrastructure.Consumer.Abstractions;
using DistributedSystem.Infrastructure.Consumer.Attributes;
using DistributedSystem.Infrastructure.Consumer.Constants;

namespace DistributedSystem.Infrastructure.Consumer.Models
{
    [BsonCollection(TableNames.Product)]
    public class ProductProjection : Document
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}