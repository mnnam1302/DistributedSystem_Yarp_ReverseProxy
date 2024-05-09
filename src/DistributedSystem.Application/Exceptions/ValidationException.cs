using DistributedSystem.Domain.Exceptions;

namespace DistributedSystem.Application.Exceptions;

public sealed class ValidationException : DomainException
{
    public ValidationException(IReadOnlyCollection<ValidationError> error)
        : base("Validation Failure", "One or more validation failures have occurred.")
    {
        Errors = error;
    }

    public IReadOnlyCollection<ValidationError> Errors { get; }
}

public record ValidationError(string PropertyName, string ErrorMessage);
