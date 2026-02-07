using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.DTOs;
using TodoList.API.Exceptions;
using TodoList.API.Models;
using TodoList.API.Services;
using TodoList.API.Tests.Data;

namespace TodoList.API.Tests.Services;

public class TaskListServiceTests
{
    [Fact]
    public async Task GetAsync_GetTaskListByPid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await SeedUserAsync(context);
        var service = new TaskListService(context);
        var taskList = await SeedTaskListAsync(context, user.Pid);

        var response = await service.GetByPidAsync(user.Pid, taskList.Pid);

        Assert.NotNull(response);
        Assert.Equal(taskList.Pid, response.Pid);
        Assert.Equal(taskList.Name, response.Name);
    }

    [Fact]
    public async Task GetAsync_GetTaskListByPid_ForUser_NotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new TaskListService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await service.GetByPidAsync(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task GetAsync_GetTaskListByPid_ForTaskList_NotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user1 = await SeedUserAsync(context);
        var user2 = await SeedUserAsync(context);
        var service = new TaskListService(context);
        var taskList = await SeedTaskListAsync(context, user1.Pid);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await service.GetByPidAsync(user2.Pid, taskList.Pid));

        Assert.Equal("Task list not found.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_CreatesTaskList_ForUser()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await SeedUserAsync(context);
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

    private static async Task<User> SeedUserAsync(AppDbContext context)
    {
        var user = new User
        {
            Pid = Guid.NewGuid(),
            Email = "a@a.com",
            Name = "Kai",
            PasswordHash = "password"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private static async Task<TaskList> SeedTaskListAsync(AppDbContext context,
        Guid userId)
    {
        var user = context.Users.Single(u => u.Pid == userId);

        var taskList = new TaskList
        {
            Pid = Guid.NewGuid(),
            Name = "My new task list",
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.TaskLists.Add(taskList);
        await context.SaveChangesAsync();

        return taskList;
    }
}