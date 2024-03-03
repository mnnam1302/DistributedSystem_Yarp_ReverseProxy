
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Query.Domain.Exceptions;
using System.Reflection;
using System.Text.Json;

namespace Query.API.Middleware
{
    internal sealed class ExceptionHandlingMiddleware : IMiddleware
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
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            int statusCode = GetStatusCode(exception);

            var response = new
            {
                title = GetTitle(exception),
                status = statusCode,
                detail = exception.Message,
                errors = GetErrors(exception)
            };

            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                EventException.EventNotFoundException => StatusCodes.Status404NotFound,

                ProductException.ProductNotFoundException => StatusCodes.Status404NotFound,

                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        
        private static string GetTitle(Exception exception) =>
            exception switch
            {
                DomainException applicationException => applicationException.Title,
                _ => "Server error"
            };

        // Note: validationException usually side Command more Query
        // Canbe implement this if want to use Error at Contract
        //private static IReadOnlyCollection<DistributedSystem.Contract.Abstractions.Shared.Error> GetErrors(Exception exception)
        //{
        //    IReadOnlyCollection<DistributedSystem.Contract.Abstractions.Shared.Error> errors = null;

        //    if (exception is Query.Application.Exceptions.ValidationException validationException)
        //    {
        //        errors = validationException.Errors;
        //    }

        //    return errors;
        //}

        private static IReadOnlyCollection<Query.Application.Exceptions.ValidationError> GetErrors(Exception exception)
        {
            IReadOnlyCollection<Query.Application.Exceptions.ValidationError> errors = null;

            if (exception is Query.Application.Exceptions.ValidationException validationException)
            {
                errors = validationException.Errors;
            }

            return errors;
        }
    }
}
