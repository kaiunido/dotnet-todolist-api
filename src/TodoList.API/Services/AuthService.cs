using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.DTOs;
using TodoList.API.Exceptions;
using TodoList.API.Models;

namespace TodoList.API.Services;

public class AuthService(
    AppDbContext context,
    ITokenService tokenService,
    IHttpContextAccessor httpContextAccessor
) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(
        UserRegisterDto userRegisterDto)
    {
        var userExists =
            await context.Users.AnyAsync(u => u.Email == userRegisterDto.Email);

        if (userExists)
        {
            throw new ConflictException("User already exists.");
        }

        var hashedPassword = HashPassword(userRegisterDto.Password);
        var user = new User
        {
            Name = userRegisterDto.Name,
            Email = userRegisterDto.Email,
            PasswordHash = hashedPassword
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return await LoginAsync(new LoginDto
        {
            Email = user.Email,
            Password = userRegisterDto.Password
        });
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user =
            await context.Users.SingleOrDefaultAsync(u =>
                u.Email == loginDto.Email);

        if (user is null ||
            !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var deviceInfo = ResolveDeviceInfo();

        await DeleteExpiredSessionsForUserAsync(user.Id);
        await DeleteSessionsForDeviceAsync(user.Id, deviceInfo);

        var token = await tokenService.GenerateTokenAsync(user, deviceInfo);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Pid = user.Pid,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<bool> LogoutAsync(Guid jti)
    {
        var session = await context.UserSessions
            .FirstOrDefaultAsync(s => s.Jti == jti);

        if (session == null)
        {
            return false;
        }

        context.UserSessions.Remove(session);
        await context.SaveChangesAsync();

        return true;
    }

    private static string HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();

        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    private string ResolveDeviceInfo(string? deviceInfoFromRequest = null)
    {
        var userAgent = httpContextAccessor.HttpContext?.Request.Headers
            .UserAgent.ToString();

        return deviceInfoFromRequest ?? userAgent ?? "Unknown Device";
    }

    private Task<int> DeleteSessionsForDeviceAsync(int userId,
        string deviceInfo,
        CancellationToken cancellationToken = default)
    {
        return context.UserSessions
            .Where(s => s.UserId == userId && s.DeviceInfo == deviceInfo)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private Task<int> DeleteExpiredSessionsForUserAsync(int userId,
        CancellationToken ct = default)
    {
        return context.UserSessions
            .Where(s => s.UserId == userId && s.ExpiresAt <= DateTime.UtcNow)
            .ExecuteDeleteAsync(ct);
    }
}