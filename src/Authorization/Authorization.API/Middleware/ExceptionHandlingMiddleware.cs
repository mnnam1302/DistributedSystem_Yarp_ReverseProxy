using Authorization.Domain.Exceptions;
using System.Text.Json;

namespace Authorization.API.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode = GetStatusCode(exception);

            var response = new
            {
                title = GetTitle(exception),
                status = statusCode,
                detail = exception.Message,
                errors = GetErrors(exception)
            };

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

        private static string GetTitle(Exception exception) =>
            exception switch
            {
                DomainException domainException => domainException.Title,
                _ => "Server Error"
            };

        private static IReadOnlyCollection<Authorization.Application.Exceptions.ValidationError> GetErrors(Exception exception)
        {
            IReadOnlyCollection<Authorization.Application.Exceptions.ValidationError> errors = null;

            if (exception is Authorization.Application.Exceptions.ValidationException validationException)
            {
                errors = validationException.Errors;
            }

            return errors;
        }
    }
}