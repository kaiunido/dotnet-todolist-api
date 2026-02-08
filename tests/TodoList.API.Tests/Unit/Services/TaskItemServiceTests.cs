using Microsoft.EntityFrameworkCore;
using TodoList.API.DTOs;
using TodoList.API.Exceptions;
using TodoList.API.Services;
using TodoList.API.Tests.Data;
using TodoList.API.Tests.Unit.Fixtures;

namespace TodoList.API.Tests.Unit.Services;

public class TaskItemServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateTaskWithSuccess()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        var service = new TaskItemService(context);

        var itemDto = new TaskItemCreateDto
        {
            Description = "  Test Task  "
        };

        var createdItem = await service.CreateAsync(
            user.Pid,
            taskList.Pid,
            itemDto
        );

        Assert.NotNull(createdItem);
        Assert.Equal("Test Task", createdItem.Description);
        Assert.NotEqual(Guid.Empty, createdItem.Pid);
        Assert.Equal(taskList.Pid, createdItem.TaskListPid);

        var count = await context.TaskItems.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_ForTaskListNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var service = new TaskItemService(context);

        var itemDto = new TaskItemCreateDto
        {
            Description = "Test Task"
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync(
                user.Pid,
                Guid.NewGuid(),
                itemDto
            )
        );

        Assert.Equal("Task list not found.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_ForUserNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        var service = new TaskItemService(context);

        var itemDto = new TaskItemCreateDto
        {
            Description = "Test Task"
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync(
                Guid.NewGuid(),
                taskList.Pid,
                itemDto
            )
        );

        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserNotOwnTaskList()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var otherUser = await UserFixture.SeedAsync(
            context,
            Guid.NewGuid(),
            "Other User",
            "otherUser@test.com",
            "password"
        );
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        var service = new TaskItemService(context);

        var itemDto = new TaskItemCreateDto
        {
            Description = "Test Task"
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync(
                otherUser.Pid,
                taskList.Pid,
                itemDto
            )
        );

        Assert.Equal("Task list not found.", ex.Message);
    }
}