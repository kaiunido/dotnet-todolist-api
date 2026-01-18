using TodoList.API.DTOs;

namespace TodoList.API.Services;

public interface IUserService
{
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
}