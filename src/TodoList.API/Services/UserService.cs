using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.DTOs;
using TodoList.API.Exceptions;

namespace TodoList.API.Services;

public class UserService(AppDbContext context) : IUserService
{
    public async Task<UserResponseDto?> GetUserByPidAsync(Guid pid)
    {
        var dbUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Pid == pid);

        return dbUser == null
            ? null
            : new UserResponseDto
            {
                Pid = dbUser.Pid,
                Name = dbUser.Name,
                Email = dbUser.Email,
                CreatedAt = dbUser.CreatedAt
            };
    }

    public async Task<UserResponseDto?> UpdateUserAsync(Guid userPid,
        UserUpdateDto userUpdateDto)
    {
        var user =
            await context.Users.FirstOrDefaultAsync(u => u.Pid == userPid);

        if (user == null)
        {
            return null;
        }

        if (userUpdateDto.Name != null)
        {
            user.Name = userUpdateDto.Name;
        }

        if (userUpdateDto.Email != null)
        {
            var emailExists = await context.Users.AnyAsync(u =>
                u.Email == userUpdateDto.Email && u.Pid != userPid);

            if (emailExists)
            {
                throw new ConflictException(
                    "Email already exists for another user.");
            }

            user.Email = userUpdateDto.Email;
        }

        await context.SaveChangesAsync();

        return await GetUserByPidAsync(userPid);
    }
}