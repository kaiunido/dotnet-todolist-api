using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.DTOs;
using TodoList.API.Models;

namespace TodoList.API.Services;

public class AuthService(
    AppDbContext _context,
    ITokenService _tokenService
) : IAuthService {
    public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto userRegisterDto)
    {
        var hashedPassword = HashPassword(userRegisterDto.Password);
        var user = new User
        {
            Name = userRegisterDto.Name,
            Email = userRegisterDto.Email,
            PasswordHash = hashedPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await LoginAsync(new LoginDto
        {
            Email = user.Email,
            Password = userRegisterDto.Password
        });
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Make login with generating token with JwtBearer
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = await _tokenService.GenerateTokenAsync(user);

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

    public Task<bool> LogoutAsync(string token)
    {
        throw new NotImplementedException();
    }

    private static string HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();

        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }
}