namespace DistributedSystem.Domain.Exceptions
{
    public class BadRequestException : DomainException
    {
        protected BadRequestException(string message)
            : base("Bad Request", message)
        {
        }
    }
}