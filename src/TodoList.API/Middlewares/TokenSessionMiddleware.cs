using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;

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
        var jtiClaim =
            context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrEmpty(jtiClaim))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // Check if the session exists on a database
        var sessionExists = await dbContext.UserSessions
            .AsNoTracking()
            .AnyAsync(s =>
                s.Jti == Guid.Parse(jtiClaim) && s.ExpiresAt > DateTime.Now);

        if (!sessionExists)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // All good, continue
        await next(context);
    }
}