using Microsoft.AspNetCore.Diagnostics;
using TodoList.API.Exceptions;

namespace TodoList.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var response = exception switch
        {
            ConflictException => (StatusCode: 409, Title: "Conflict"),
            NotFoundException => (StatusCode: 404, Title: "Not Found"),
            _ => (StatusCode: 500, Title: "Internal Server Error")
        };

        httpContext.Response.StatusCode = response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new {
                response.Title,
                exception.Message
            },
            cancellationToken
        );

        return true;
    }
}