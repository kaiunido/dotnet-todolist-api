using System.Net;
using System.Net.Http.Json;
using TodoList.API.DTOs;
using TodoList.API.Tests.Infrastructure;
using TodoList.API.Tests.Integration.Fixtures;

namespace TodoList.API.Tests.Integration;

public class
    TaskListControllerIntegrationTests(
        CustomWebAppApplicationFactory factory
    ) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Get_TaskLists_WithoutAuth_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var client = CreateClient();

        var response = await client.GetAsync("/api/task-lists");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task
        Get_TaskLists_WithAuthButNoSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAsync();

        var client = CreateClient(true);
        var response = await client.GetAsync("/api/task-lists");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_TaskListsByPid_ShouldReturnOk()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();

        var client = CreateClient(true);

        var taskLists = new TaskListFixture(WithDb);
        var createdTaskList = await taskLists.CreateAsync(user.Pid);

        var response =
            await client.GetAsync($"/api/task-lists/{createdTaskList.Pid}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var listedTaskList =
            await response.Content.ReadFromJsonAsync<TaskListResponseDto>();
        Assert.NotNull(listedTaskList);
        Assert.Equal(createdTaskList.Pid, listedTaskList.Pid);
        Assert.Equal(createdTaskList.Name, listedTaskList.Name);
    }

    [Fact]
    public async Task
        Post_TaskList_WithValidSession_ShouldCreateAndReturnCreated()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();

        var client = CreateClient(true);

        var dto = new TaskListCreateDto
        {
            Name = "Market List"
        };

        var response = await client.PostAsJsonAsync("/api/task-lists", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<TaskListResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("Market List", body.Name);
        Assert.NotEqual(Guid.Empty, body.Pid);
    }
}