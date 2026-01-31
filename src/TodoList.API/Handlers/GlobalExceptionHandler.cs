using Microsoft.AspNetCore.Diagnostics;
using TodoList.API.Exceptions;

namespace TodoList.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            // Application domain exceptions
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

            // Validation exceptions
            FluentValidation.ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
            ArgumentException or FormatException => (StatusCodes.Status400BadRequest, "Bad Request"),

            // Other exceptions
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                title,
                message = exception.Message
            },
            cancellationToken
        );

        return true;
    }
}