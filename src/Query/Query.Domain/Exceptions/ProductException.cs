namespace Query.Domain.Exceptions;

public static class ProductException
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(Guid id)
            : base($"The product with id: {id} was not found.")
        {
        }
    }
}
