namespace DistributedSystem.Domain.Exceptions;

public class NotFoundException : DomainException
{
    protected NotFoundException(string message)
        : base("Not Found", message)
    {
    }
}
