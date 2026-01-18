using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.DTOs;

namespace TodoList.API.Services;

public class UserService(AppDbContext context) : IUserService
{
    public async Task<UserResponseDto?> GetUserByPidAsync(Guid pid)
    {
        var dbUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Pid == pid);

        return dbUser == null ? null : new UserResponseDto
        {
            Pid = dbUser.Pid,
            Name = dbUser.Name,
            Email = dbUser.Email,
            CreatedAt = dbUser.CreatedAt
        };
    }
}