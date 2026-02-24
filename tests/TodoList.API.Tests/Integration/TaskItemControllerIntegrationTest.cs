using System.Net;
using System.Net.Http.Json;
using TodoList.API.DTOs;
using TodoList.API.Tests.Infrastructure;
using TodoList.API.Tests.Integration.Fixtures;

namespace TodoList.API.Tests.Integration;

public class TaskItemControllerIntegrationTest(
    CustomWebAppApplicationFactory factory
) : IntegrationTestBase(factory)
{
    private TaskItemFixture TaskItemFixture => new(WithDb);
    private TaskListFixture TaskListFixture => new(WithDb);

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user.Pid);
        await TaskItemFixture.CreateAsync(taskList.Pid);

        var response =
            await client.GetAsync($"/api/task-lists/{taskList.Pid}/items");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var taskItems = await response.Content
            .ReadFromJsonAsync<PaginationResponse<TaskItemResponseDto>>();

        Assert.NotNull(taskItems);
        Assert.Single(taskItems.Data);
        Assert.Equal(1, taskItems.Meta.TotalPages);
        Assert.Equal(1, taskItems.Meta.Page);
        Assert.Equal(10, taskItems.Meta.PerPage);
        Assert.Equal(
            $"/api/task-lists/{taskList.Pid}/items?page=1&perPage=10",
            taskItems.Meta.Links.Self
        );
        Assert.Null(taskItems.Meta.Links.Prev);
        Assert.Null(taskItems.Meta.Links.Next);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user.Pid);

        var response =
            await client.GetAsync($"/api/task-lists/{taskList.Pid}/items");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var taskItems = await response.Content
            .ReadFromJsonAsync<PaginationResponse<TaskItemResponseDto>>();

        Assert.NotNull(taskItems);
        Assert.Empty(taskItems.Data);
        Assert.Equal(1, taskItems.Meta.TotalPages);
        Assert.Equal(1, taskItems.Meta.Page);
        Assert.Equal(10, taskItems.Meta.PerPage);
    }

    [Fact]
    public async Task GetAll_ShouldReturnNotFound()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var response =
            await client.GetAsync($"/api/task-lists/{Guid.NewGuid()}/items");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var client = CreateClient();

        var response =
            await client.GetAsync($"/api/task-lists/{Guid.NewGuid()}/items");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_NoSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAsync();
        var client = CreateClient(true);

        var response =
            await client.GetAsync($"/api/task-lists/{Guid.NewGuid()}/items");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}