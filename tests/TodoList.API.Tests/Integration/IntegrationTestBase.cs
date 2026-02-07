using Microsoft.Extensions.DependencyInjection;
using TodoList.API.Data;
using TodoList.API.Models;
using TodoList.API.Tests.Infrastructure;

namespace TodoList.API.Tests.Integration;

public class IntegrationTestBase : IClassFixture<CustomWebAppApplicationFactory>
{
    private readonly CustomWebAppApplicationFactory Factory;

    protected IntegrationTestBase(CustomWebAppApplicationFactory factory)
    {
        Factory = factory;
    }

    protected HttpClient CreateClient(bool authenticated = false)
    {
        var client = Factory.CreateClient();

        if (authenticated)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Test");
        }

        return client;
    }

    protected async Task WithDb(Func<AppDbContext, Task> action)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await action(db);
        await db.SaveChangesAsync();
    }

    protected async Task ResetDbAsync(Action<AppDbContext>? customize = null)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.EnsureCreatedAsync();

        db.TaskLists.RemoveRange(db.TaskLists);
        db.UserSessions.RemoveRange(db.UserSessions);
        db.Users.RemoveRange(db.Users);

        customize?.Invoke(db);

        await db.SaveChangesAsync();
    }

    protected async Task<User> SeedUserAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = new User
        {
            Pid = TestAuthHandler.DefaultUserPid,
            Name = TestAuthHandler.DefaultName,
            Email = TestAuthHandler.DefaultEmail,
            PasswordHash = "password",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    protected async Task<User> SeedUserAndSessionAsync()
    {
        var user = await SeedUserAsync();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.UserSessions.Add(new UserSession
        {
            UserId = user.Id,
            Jti = TestAuthHandler.DefaultJti,
            DeviceInfo = "Test Device",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        return user;
    }
}