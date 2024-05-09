namespace DistributedSystem.Domain.Exceptions;

public static class ProductException
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(Guid productId)
            : base($"The product with id {productId} was not found.")
        {
        }
    }

    public class ProductFieldException : NotFoundException
    {
        public ProductFieldException(string productField)
            : base($"The product with field {productField} is not correct.")
        {
        }
    }
}
