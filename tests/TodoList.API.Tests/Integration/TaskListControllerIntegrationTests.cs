using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
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
    public async Task GetAll_WithoutAuth_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var client = CreateClient();

        var response = await client.GetAsync("/api/task-lists");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithAuthButNoSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAsync();

        var client = CreateClient(true);
        var response = await client.GetAsync("/api/task-lists");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();

        var client = CreateClient(true);

        await TaskListFixture.CreateAsync(user.Pid);

        var response = await client.GetAsync("/api/task-lists");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var taskLists = await response.Content
            .ReadFromJsonAsync<PaginationResponse<TaskListResponseDto>>();
        Assert.NotNull(taskLists);
        Assert.Single(taskLists.Data);
        Assert.Equal(1, taskLists.Meta.TotalPages);
        Assert.Equal(1, taskLists.Meta.Page);
        Assert.Equal(10, taskLists.Meta.PerPage);
        Assert.Equal("/api/task-lists?page=1&perPage=10",
            taskLists.Meta.Links.Self);
        Assert.Null(taskLists.Meta.Links.Prev);
        Assert.Null(taskLists.Meta.Links.Next);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();

        var client = CreateClient(true);

        var response = await client.GetAsync("/api/task-lists");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var taskLists = await response.Content
            .ReadFromJsonAsync<PaginationResponse<TaskListResponseDto>>();
        Assert.NotNull(taskLists);
        Assert.Empty(taskLists.Data);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOnlyCurrentUserItems()
    {
        await ResetDbAsync();
        var user1 = await SeedUserAndSessionAsync();

        var user2 = await UserFixture.CreateAsync();
        await TaskListFixture.CreateAsync(user1.Pid, "Mine");
        await TaskListFixture.CreateAsync(user2.Pid, "Not mine");

        var client = CreateClient(true);
        var response = await client.GetAsync("/api/task-lists");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<PaginationResponse<TaskListResponseDto>>();
        Assert.NotNull(body);
        Assert.Single(body.Data);
        Assert.Equal("Mine", body.Data[0].Name);
        Assert.Equal(1, body.Meta.TotalItems);
    }

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
    public async Task GetByPid_WithoutAuth_ShouldReturnNotFound()
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

    [Fact]
    public async Task Update_WithValidSession_ShouldUpdateAndReturnOk()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

        var dto = new TaskListUpdateDto
        {
            Name = "Updated List"
        };

        var response = await client.PatchAsJsonAsync(
            $"/api/task-lists/{createdTaskList.Pid}", dto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<TaskListResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("Updated List", body.Name);
    }

    [Fact]
    public async Task Update_WithoutSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var user = await SeedUserAsync();
        var client = CreateClient(true);
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

        var dto = new TaskListUpdateDto
        {
            Name = "Updated List"
        };

        var response = await client.PatchAsJsonAsync(
            $"/api/task-lists/{createdTaskList.Pid}", dto);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithoutAuth_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient();
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

        var dto = new TaskListUpdateDto
        {
            Name = "Updated List"
        };

        var response = await client.PatchAsJsonAsync(
            $"/api/task-lists/{createdTaskList.Pid}", dto);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

        var response =
            await client.DeleteAsync($"/api/task-lists/{createdTaskList.Pid}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var response2 =
            await client.DeleteAsync($"/api/task-lists/{createdTaskList.Pid}");
        Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);
    }

    [Fact]
    public async Task Delete_WithoutSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var user = await SeedUserAsync();
        var client = CreateClient(true);
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

        var response =
            await client.DeleteAsync($"/api/task-lists/{createdTaskList.Pid}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithoutAuth_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient();
        var createdTaskList = await TaskListFixture.CreateAsync(user.Pid);

        var response =
            await client.DeleteAsync($"/api/task-lists/{createdTaskList.Pid}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithValidSessionButNotOwner_ShouldReturnNotFound()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();
        var user2 = await UserFixture.CreateAsync();
        var client = CreateClient(true);
        var createdTaskList = await TaskListFixture.CreateAsync(user2.Pid);

        var response =
            await client.DeleteAsync($"/api/task-lists/{createdTaskList.Pid}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var stillExists = false;
        await WithDb(async db =>
        {
            stillExists = await db.TaskLists
                .AnyAsync(tl => tl.Pid == createdTaskList.Pid);
        });

        Assert.True(stillExists);
    }
}