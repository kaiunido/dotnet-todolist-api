using TodoList.API.DTOs;

namespace TodoList.API.Services;

public interface IUserService
{
    Task<UserResponseDto?> GetUserByPidAsync(Guid pid);

    Task<UserResponseDto?> UpdateUserAsync(Guid userPid,
        UserUpdateDto userUpdateDto);
}