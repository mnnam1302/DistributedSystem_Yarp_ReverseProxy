﻿using DistributedSystem.Domain.Exceptions;
using System.Text.Json;

namespace DistributedSystem.API.Middleware;

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
            // Nếu trong next() có lỗi thì ngoài catch sẽ bắt được và nó sẽ handle cái lỗi
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

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
            // switch statement

            // Trường hợp throw ở domain => Domain driven design
            ProductException.ProductFieldException => StatusCodes.Status406NotAcceptable, // Should be remove later

            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            //Application.Exceptions.ValidationException => StatusCodes.Status422UnprocessableEntity,
            //FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            FormatException => StatusCodes.Status422UnprocessableEntity,
            // Trường hợp mặc định, nếu không có th nào map ở trên thì trả về 500
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            DomainException applicationException => applicationException.Title,
            _ => "Server error"
        };

    private static IReadOnlyCollection<DistributedSystem.Application.Exceptions.ValidationError> GetErrors(Exception exception)
    {
        IReadOnlyCollection<DistributedSystem.Application.Exceptions.ValidationError> errors = null;

        if (exception is DistributedSystem.Application.Exceptions.ValidationException validationException)
        {
            errors = validationException.Errors;
        }

        return errors;
    }
}
