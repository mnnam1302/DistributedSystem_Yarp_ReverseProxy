namespace DistributedSystem.Infrastructure.Consumer.Exceptions
{
    public class ConsumerException : Exception
    {
        public ConsumerException(string title, string message)
            : base(message)
        {
            Title = title;
        }

        public string Title { get; }
    }
}