namespace DistributedSystem.Infrastructure.Consumer.Exceptions
{
    public static class ConsumerProductException
    {
        public class ConsumerProductNotFoundException : ConsumerNotFoundException
        {
            public ConsumerProductNotFoundException(Guid productId)
                : base($"The product projection with id {productId} was not found.")
            {
            }
        }
    }
}