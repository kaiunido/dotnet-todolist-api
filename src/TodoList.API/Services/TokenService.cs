using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoList.API.Configurations;
using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Services;

public class TokenService(
    IOptions<JwtSettings> jwtOptions,
    IHttpContextAccessor httpContextAccessor,
    AppDbContext context
) : ITokenService
{

    private readonly JwtSettings _settings = jwtOptions.Value;

    public async Task<string> GenerateTokenAsync(User user, string? deviceInfo = null)
    {
        var jti = Guid.NewGuid();
        var expires = DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes);
        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown Device";
        var keyBytes = Encoding.UTF8.GetBytes(_settings.Key);

        var session = new UserSession
        {
            Jti = jti,
            UserId = user.Id,
            DeviceInfo = deviceInfo ?? userAgent,
            IpAddress = ipAddress,
            ExpiresAt = expires
        };

        context.UserSessions.Add(session);
        await context.SaveChangesAsync();

        var key = new SymmetricSecurityKey(keyBytes);

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Pid.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}