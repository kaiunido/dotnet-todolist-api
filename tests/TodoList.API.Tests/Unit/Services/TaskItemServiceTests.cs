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
    public async Task GetAllAsync_ShouldReturnTaskItems()
    {
        await using var context = TestDbContextFactory.Create();

        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);
        await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);

        var user2 = await UserFixture.SeedDefaultAsync(context);
        var taskList2 =
            await TaskListFixture.SeedDefaultAsync(context, user2.Id);
        await TaskItemFixture.SeedDefaultAsync(context, taskList2.Id);

        var service = new TaskItemService(context);

        var taskItems = await service.GetAllAsync(user.Pid, taskList.Pid);

        Assert.Equal(2, taskItems.Data.Count);
        Assert.Equal(2, taskItems.Meta.TotalItems);
        Assert.All(taskItems.Data,
            i => Assert.Equal(taskList.Pid, i.TaskListPid));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);

        var service = new TaskItemService(context);
        var taskItems = await service.GetAllAsync(user.Pid, taskList.Pid);

        Assert.Empty(taskItems.Data);
        Assert.Equal(0, taskItems.Meta.TotalItems);
        Assert.Equal(1, taskItems.Meta.TotalPages);
        Assert.Equal(1, taskItems.Meta.Page);
        Assert.Equal(10, taskItems.Meta.PerPage);
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrow_ForUserNotFound()
    {
        await using var context = TestDbContextFactory.Create();

        var service = new TaskItemService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetAllAsync(
                Guid.NewGuid(),
                Guid.NewGuid()
            )
        );

        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrow_ForTaskListNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);

        var service = new TaskItemService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetAllAsync(
                user.Pid,
                Guid.NewGuid()
            )
        );

        Assert.Equal("Task list not found.", ex.Message);
    }

    [Fact]
    public async Task GetAllAsync_ShouldDefaultPageAndPerPage_WhenInvalid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);

        var service = new TaskItemService(context);

        var taskItems = await service.GetAllAsync(user.Pid, taskList.Pid, 0, 0);

        Assert.Single(taskItems.Data);
        Assert.Equal(1, taskItems.Meta.Page);
        Assert.Equal(10, taskItems.Meta.PerPage);
    }

    [Fact]
    public async Task GetAllAsync_ShouldHavePagination()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);
        await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);
        await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);

        var service = new TaskItemService(context);

        var taskItems = await service.GetAllAsync(user.Pid, taskList.Pid, 2, 1);

        Assert.Single(taskItems.Data);
        Assert.Equal(2, taskItems.Meta.Page);
        Assert.Equal(1, taskItems.Meta.PerPage);
        Assert.Equal(3, taskItems.Meta.TotalPages);
        Assert.Equal(3, taskItems.Meta.TotalItems);
        Assert.Equal($"/api/task-lists/{taskList.Pid}/tasks?page=2&perPage=1",
            taskItems.Meta.Links.Self);
        Assert.Equal($"/api/task-lists/{taskList.Pid}/tasks?page=3&perPage=1",
            taskItems.Meta.Links.Next);
        Assert.Equal($"/api/task-lists/{taskList.Pid}/tasks?page=1&perPage=1",
            taskItems.Meta.Links.Prev);
    }

    [Fact]
    public async Task GetByPidAsync_ShouldReturnTaskItem()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        var taskItem =
            await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);

        var service = new TaskItemService(context);

        var taskItemDto =
            await service.GetByPidAsync(user.Pid, taskList.Pid, taskItem.Pid);

        Assert.NotNull(taskItemDto);
        Assert.Equal(taskItem.Pid, taskItemDto.Pid);
        Assert.Equal(taskList.Pid, taskItemDto.TaskListPid);
        Assert.Equal(taskItem.Description, taskItemDto.Description);
    }

    [Fact]
    public async Task GetByPidAsync_ShouldThrow_ForUserNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        var taskItem =
            await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);
        var service = new TaskItemService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByPidAsync(Guid.NewGuid(), taskList.Pid, taskItem.Pid)
        );

        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task GetByPidAsync_ShouldThrow_ForTaskListNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        var taskItem =
            await TaskItemFixture.SeedDefaultAsync(context, taskList.Id);
        var service = new TaskItemService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByPidAsync(user.Pid, Guid.NewGuid(), taskItem.Pid)
        );

        Assert.Equal("Task list not found.", ex.Message);
    }

    [Fact]
    public async Task GetByPidAsync_ShouldThrow_ForTaskNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);
        var taskList = await TaskListFixture.SeedDefaultAsync(context, user.Id);
        var service = new TaskItemService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByPidAsync(user.Pid, taskList.Pid, Guid.NewGuid())
        );

        Assert.Equal("Task item not found.", ex.Message);
    }

    [Fact]
    public async Task GetByPidAsync_ShouldThrow_ForTaskFromAnotherList()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await UserFixture.SeedDefaultAsync(context);

        var list1 = await TaskListFixture.SeedAsync(context, user.Id, "List 1");
        var list2 = await TaskListFixture.SeedAsync(context, user.Id, "List 2");

        var itemInList2 = await TaskItemFixture.SeedAsync(context, list2.Id, "Item 2");

        var service = new TaskItemService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByPidAsync(user.Pid, list1.Pid, itemInList2.Pid)
        );

        Assert.Equal("Task item not found.", ex.Message);
    }

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
        var otherUser = await UserFixture.SeedDefaultAsync(context);
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