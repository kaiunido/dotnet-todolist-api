using TodoList.API.DTOs;

namespace TodoList.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto userRegisterDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> LogoutAsync(string token);
}