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
        var pid = Guid.NewGuid();

        return await SeedAsync(
            context,
            pid,
            $"Test User {pid}",
            $"test{pid}@test.com",
            "password"
        );
    }
}