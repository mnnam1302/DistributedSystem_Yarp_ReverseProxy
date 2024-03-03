using Query.Domain.Exceptions;

namespace Query.Application.Exceptions
{
    public class ValidationException : DomainException
    {
        //public ValidationException(IReadOnlyCollection<DistributedSystem.Contract.Abstractions.Shared.Error> errors)
        //    : base("Validation Failure", "One or more validation failures have occurred.")
        //{
        //    Errors = errors;
        //}

        //public IReadOnlyCollection<DistributedSystem.Contract.Abstractions.Shared.Error> Errors { get; set; }

        public ValidationException(IReadOnlyCollection<ValidationError> errors)
           : base("Validation Failure", "One or more validation failures have occurred.")
        {
            Errors = errors;
        }

        public IReadOnlyCollection<ValidationError> Errors { get; set; }
    }

    public record ValidationError(string PropertyName, string ErrorMessage);
}