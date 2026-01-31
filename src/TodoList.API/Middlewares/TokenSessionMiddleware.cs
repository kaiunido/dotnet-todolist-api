using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.Extensions;

namespace TodoList.API.Middlewares;

public class TokenSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // If user is not authenticated continue
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        // If a token is missing or invalid, return 401 Unauthorized
        if (!context.User.TryGetJti(out var jti))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new
                {
                    title = "Unauthorized",
                    message = "Invalid token identifier."
                }
            );
            return;
        }

        // Check if the session exists on a database
        var sessionExists = await dbContext.UserSessions
            .AsNoTracking()
            .AnyAsync(s =>
                s.Jti == jti && s.ExpiresAt > DateTime.UtcNow);

        if (!sessionExists)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new
                {
                    title = "Unauthorized",
                    message = "Session expired or revoked."
                }
            );
            return;
        }

        // All good, continue
        await next(context);
    }
}