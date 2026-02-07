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
    public async Task GetAllAsync_ShouldReturnTaskLists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await SeedUserAsync(context);
        var user2 = await SeedUserAsync(context);
        var service = new TaskListService(context);
        await SeedTaskListAsync(context, user.Pid);
        await SeedTaskListAsync(context, user.Pid);
        await SeedTaskListAsync(context, user2.Pid);

        var taskLists = await service.GetAllAsync(user.Pid);

        Assert.Equal(2, taskLists.Data.Count);
        Assert.Equal(2, taskLists.Meta.TotalItems);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await SeedUserAsync(context);

        var service = new TaskListService(context);
        var taskLists = await service.GetAllAsync(user.Pid);

        Assert.Empty(taskLists.Data);
        Assert.Equal(0, taskLists.Meta.TotalItems);
        Assert.Equal(1, taskLists.Meta.TotalPages);
        Assert.Equal(1, taskLists.Meta.Page);
        Assert.Equal(10, taskLists.Meta.PerPage);
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrowNotFoundException_ForUser()
    {
        await using var context = TestDbContextFactory.Create();

        var service = new TaskListService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await service.GetAllAsync(Guid.NewGuid()));
        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task GetAllAsync_ShouldDefaultPageAndPerPage_WhenInvalid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await SeedUserAsync(context);
        var service = new TaskListService(context);
        await SeedTaskListAsync(context, user.Pid);

        var taskLists = await service.GetAllAsync(user.Pid, 0, 0);
        Assert.Single(taskLists.Data);
        Assert.Equal(1, taskLists.Meta.Page);
        Assert.Equal(10, taskLists.Meta.PerPage);
    }

    [Fact]
    public async Task GetAllAsync_ShouldHavePagination()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await SeedUserAsync(context);
        var user2 = await SeedUserAsync(context);
        var service = new TaskListService(context);
        await SeedTaskListAsync(context, user.Pid);
        await SeedTaskListAsync(context, user.Pid);
        await SeedTaskListAsync(context, user.Pid);
        await SeedTaskListAsync(context, user2.Pid);

        var taskLists = await service.GetAllAsync(user.Pid, 2, 1);

        Assert.Single(taskLists.Data);
        Assert.Equal(3, taskLists.Meta.TotalItems);
        Assert.Equal(3, taskLists.Meta.TotalPages);
        Assert.Equal(2, taskLists.Meta.Page);
        Assert.Equal(1, taskLists.Meta.PerPage);
        Assert.Equal("/api/task-lists?page=2&perPage=1",
            taskLists.Meta.Links.Self);
        Assert.Equal("/api/task-lists?page=3&perPage=1",
            taskLists.Meta.Links.Next);
        Assert.Equal("/api/task-lists?page=1&perPage=1",
            taskLists.Meta.Links.Prev);
    }

    [Fact]
    public async Task GetByPidAsync()
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
    public async Task
        GetByPidAsync_ShouldThrowNotFoundException_ForUser()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new TaskListService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await service.GetByPidAsync(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task GetByPidAsync_ShouldThrowNotFoundException_ForTaskList()
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
    public async Task CreateAsync()
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
        var userPid = Guid.NewGuid();

        var user = new User
        {
            Pid = userPid,
            Email = $"{userPid}@a.com",
            Name = $"User {userPid}",
            PasswordHash = "password"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private static async Task<TaskList> SeedTaskListAsync(AppDbContext context,
        Guid userPid)
    {
        var user = context.Users.Single(u => u.Pid == userPid);

        var taskListPid = Guid.NewGuid();

        var taskList = new TaskList(user.Id, $"My task list {taskListPid}");

        context.TaskLists.Add(taskList);
        await context.SaveChangesAsync();

        return taskList;
    }
}