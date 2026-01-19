using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TodoList.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserPid(this ClaimsPrincipal user,
        out Guid userPid)
    {
        userPid = Guid.Empty;

        var pidClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(pidClaim, out userPid);
    }

    public static bool TryGetJti(this ClaimsPrincipal user, out Guid jti)
    {
        jti = Guid.Empty;

        var jtiClaim = user.FindFirstValue(JwtRegisteredClaimNames.Jti);

        return Guid.TryParse(jtiClaim, out jti);
    }
}