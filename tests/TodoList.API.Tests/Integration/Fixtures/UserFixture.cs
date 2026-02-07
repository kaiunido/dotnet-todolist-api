using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Tests.Integration.Fixtures;

public class UserFixture(
    Func<Func<AppDbContext, Task>, Task> withDb
)
{
    public async Task<User> CreateAsync(
        string name = "Test User", string? email = null)
    {
        User? entity = null;

        await withDb(db =>
        {
            var userPid = Guid.NewGuid();

            entity = new User
            {
                Pid = userPid,
                Name = name,
                Email = email ?? $"{userPid}@test.com",
                PasswordHash = "password",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.Users.Add(entity);
            return Task.CompletedTask;
        });

        return entity ??
               throw new InvalidOperationException(
                   "UserFixture failed to create entity.");
    }
}