using Microsoft.EntityFrameworkCore;
using TodoList.API.DTOs;
using TodoList.API.Models;
using TodoList.API.Services;
using TodoList.API.Tests.Data;

namespace TodoList.API.Tests.Services;

public class TaskListServiceTests
{
    [Fact]
    public async Task CreateAsync_CreatesTaskList_ForUser()
    {
        await using var context = TestDbContextFactory.Create();

        var user = new User
        {
            Pid = Guid.NewGuid(),
            Email = "a@a.com",
            Name = "Kai",
            PasswordHash = "password"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new TaskListService(context);

        var created = await service.CreateAsync(
            user.Pid,
            new TaskListCreateDto { Name = "Test List" }
        );

        Assert.NotNull(created);
        Assert.Equal("Test List", created.Name);

        var persisted = await context.TaskLists.SingleAsync();
        Assert.Equal(user.Id, persisted.UserId);
        Assert.Equal("Test List", persisted.Name);
    }
}