namespace Query.Domain.Exceptions;

public static class EventException
{
    public class EventNotFoundException : NotFoundException
    {
        public EventNotFoundException(Guid id)
            : base($"The event with {id} was not found.")
        {
        }
    }
}
