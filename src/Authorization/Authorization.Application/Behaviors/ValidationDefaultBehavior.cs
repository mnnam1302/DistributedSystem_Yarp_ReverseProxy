using FluentValidation;
using MediatR;

namespace Authorization.Application.Behaviors;

public class ValidationDefaultBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationDefaultBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errorDictionary = _validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(x => new { x.PropertyName, x.ErrorMessage })
            .Select(x => x.FirstOrDefault())
            .ToList();

        // if (errorDictionary.Any())
        if (errorDictionary.Count > 0)
        {
            // Kiểm tra xem: Chúng ta có những cái rule, validation như người quản lý, họ sẽ check người đi vào
            // request đi vào và người đi ra response
            // Sẽ có 2 option khi ông đi vào gặp lỗi:
            //      Gặp lỗi xử lý luôn
            //      Ném lỗi ra ngoài. Bên ngoài có một Middleware Global Exception Handler sẽ xử lý
            throw new ValidationException(errorDictionary);
        }

        return await next();
    }
}
