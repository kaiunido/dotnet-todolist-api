using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Tests.Unit.Fixtures;

public class UserFixture
{
    public static async Task<User> SeedAsync(
        AppDbContext context,
        Guid pid,
        string name,
        string email,
        string passwordHash
    )
    {
        var user = new User
        {
            Pid = pid,
            Email = email,
            Name = name,
            PasswordHash = passwordHash
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public static async Task<User> SeedDefaultAsync(AppDbContext context)
    {
        return await SeedAsync(
            context,
            Guid.NewGuid(),
            "Test User",
            "test@test.com",
            "password"
        );
    }
}