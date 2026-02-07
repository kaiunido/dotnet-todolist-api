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
    private TaskListFixture TaskListFixture => new(WithDb);
    private UserFixture UserFixture => new(WithDb);

    [Fact]
    public async Task Get_WithoutAuth_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var client = CreateClient();

        var response = await client.GetAsync("/api/task-lists");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task
        Get_WithAuthButNoSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAsync();

        var client = CreateClient(true);
        var response = await client.GetAsync("/api/task-lists");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByPid_ShouldReturnOk()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

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
    public async Task GetByPid_WithoutSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAsync();

        var client = CreateClient(true);
        var response =
            await client.GetAsync($"/api/task-lists/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByPid_WithAuthButNoAccess_ShouldReturnNotFound()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();

        var user = await UserFixture.CreateAsync();
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

        var client = CreateClient(true);
        var response =
            await client.GetAsync($"/api/task-lists/{createdTaskList.Pid}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task
        Post_WithValidSession_ShouldCreateAndReturnCreated()
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