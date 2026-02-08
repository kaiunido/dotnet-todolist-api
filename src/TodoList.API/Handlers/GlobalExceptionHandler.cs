using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using TodoList.API.Exceptions;

namespace TodoList.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, title, message) = exception switch
        {
            // Application domain exceptions
            ConflictException ex => (
                StatusCodes.Status409Conflict,
                "Conflict",
                ex.Message
            ),
            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                ex.Title ?? "Not Found",
                ex.Message
            ),
            UnauthorizedAccessException ex => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ex.Message
            ),

            // Validation exceptions
            ValidationException ex => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                ex.Message
            ),
            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message
            ),
            FormatException ex => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message
            ),

            // Other exceptions
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later or contact support."
            )
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(
            new { title, message },
            cancellationToken
        );

        return true;
    }
}