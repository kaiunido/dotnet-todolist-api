using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Services;

public class TokenService(
    IConfiguration config,
    IHttpContextAccessor httpContextAccessor,
    AppDbContext _context
) : ITokenService {
    public async Task<string> GenerateTokenAsync(User user, string? deviceInfo = null)
    {
        var jti = Guid.NewGuid();
        var expires = DateTime.Now.AddMinutes(double.Parse(
            Environment.ExpandEnvironmentVariables(config["Jwt:ExpireMinutes"] ?? "60")
        ));
        var ipAdress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown Device";

        var session = new UserSession
        {
            Jti = jti,
            UserId = user.Id,
            DeviceInfo = deviceInfo ?? userAgent,
            IpAddress = ipAdress,
            ExpiresAt = expires
        };

        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            Environment.ExpandEnvironmentVariables(config["Jwt:Key"] ?? "")
        ));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Pid.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()),
            new Claim("name", user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: Environment.ExpandEnvironmentVariables(config["Jwt:Issuer"] ?? ""),
            audience: Environment.ExpandEnvironmentVariables(config["Jwt:Audience"] ?? ""),
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}