using TodoList.API.Models;

namespace TodoList.API.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user, string? deviceInfo = null);
}