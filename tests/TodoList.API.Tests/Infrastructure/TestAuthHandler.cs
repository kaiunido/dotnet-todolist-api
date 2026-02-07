using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TodoList.API.Tests.Infrastructure;

public class
    TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger,
    encoder)
{
    public const string DefaultName = "Test User";
    public const string DefaultEmail = "test@local.dev";

    public static readonly Guid DefaultUserPid =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid DefaultJti =
        Guid.Parse("11111111-1111-1111-1111-111111111123");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, DefaultUserPid.ToString()),
            new(ClaimTypes.Email, DefaultEmail),
            new(ClaimTypes.Name, "Test User"),
            new("jti", DefaultJti.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}