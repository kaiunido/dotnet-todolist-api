using TodoList.API.Models;

namespace TodoList.API.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}