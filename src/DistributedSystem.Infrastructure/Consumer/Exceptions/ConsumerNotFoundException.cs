namespace DistributedSystem.Infrastructure.Consumer.Exceptions
{
    public class ConsumerNotFoundException : ConsumerException
    {
        public ConsumerNotFoundException(string message) 
            : base("Consumer Not Found", message)
        {
        }
    }
}