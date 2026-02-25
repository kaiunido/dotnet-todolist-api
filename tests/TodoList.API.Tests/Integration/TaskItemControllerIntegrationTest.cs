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
    private UserFixture UserFixture => new(WithDb);

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

    [Fact]
    public async Task GetByPid_ShouldReturnOk()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user.Pid);
        var taskItem = await TaskItemFixture.CreateAsync(taskList.Pid);

        var response =
            await client.GetAsync(
                $"/api/task-lists/{taskList.Pid}/items/{taskItem.Pid}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var taskItemResponse =
            await response.Content.ReadFromJsonAsync<TaskItemResponseDto>();

        Assert.NotNull(taskItemResponse);
        Assert.Equal(taskItem.Pid, taskItemResponse.Pid);
        Assert.Equal(taskItem.Description, taskItemResponse.Description);
    }

    [Fact]
    public async Task GetByPid_ShouldReturnNotFound()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user.Pid);

        var response =
            await client.GetAsync(
                $"/api/task-lists/{taskList.Pid}/items/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error =
            await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Not Found", error.Title);
        Assert.Equal("Task item not found.", error.Message);
    }

    [Fact]
    public async Task GetByPid_OwnItemOtherList_ShouldReturnNotFound()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user.Pid);
        var taskList2 = await TaskListFixture.CreateAsync(user.Pid);
        var taskItem = await TaskItemFixture.CreateAsync(taskList.Pid);

        var response =
            await client.GetAsync(
                $"/api/task-lists/{taskList2.Pid}/items/{taskItem.Pid}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error =
            await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Not Found", error.Title);
        Assert.Equal("Task item not found.", error.Message);
    }

    [Fact]
    public async Task GetByPid_OtherUserItem_ShouldReturnNotFound()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();
        var user2 = await UserFixture.CreateAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user2.Pid);
        var taskItem = await TaskItemFixture.CreateAsync(taskList.Pid);

        var response =
            await client.GetAsync(
                $"/api/task-lists/{taskList.Pid}/items/{taskItem.Pid}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error =
            await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Not Found", error.Title);
        Assert.Equal("Task list not found.", error.Message);
    }

    [Fact]
    public async Task GetByPid_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();
        var client = CreateClient();

        var response =
            await client.GetAsync(
                $"/api/task-lists/{Guid.NewGuid()}/items/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByPid_NoSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAsync();
        var client = CreateClient(true);

        var response =
            await client.GetAsync(
                $"/api/task-lists/{Guid.NewGuid()}/items/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user.Pid);

        var dto = new TaskItemCreateDto
        {
            Description = "Test Task Item"
        };

        var response =
            await client.PostAsJsonAsync(
                $"/api/task-lists/{taskList.Pid}/items", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var taskItem =
            await response.Content.ReadFromJsonAsync<TaskItemResponseDto>();

        Assert.NotNull(taskItem);
        Assert.NotEqual(Guid.Empty, taskItem.Pid);
        Assert.Equal(dto.Description, taskItem.Description);
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var dto = new TaskItemCreateDto
        {
            Description = "Test Task Item"
        };

        var response =
            await client.PostAsJsonAsync(
                $"/api/task-lists/{Guid.NewGuid()}/items", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error =
            await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Not Found", error.Title);
        Assert.Equal("Task list not found.", error.Message);
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAndSessionAsync();
        var client = CreateClient();

        var dto = new TaskItemCreateDto
        {
            Description = "Test Task Item"
        };

        var response =
            await client.PostAsJsonAsync(
                $"/api/task-lists/{Guid.NewGuid()}/items", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_NoSession_ShouldReturnUnauthorized()
    {
        await ResetDbAsync();
        await SeedUserAsync();
        var client = CreateClient(true);

        var dto = new TaskItemCreateDto
        {
            Description = "Test Task Item"
        };

        var response =
            await client.PostAsJsonAsync(
                $"/api/task-lists/{Guid.NewGuid()}/items", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidDto_ShouldReturnBadRequest()
    {
        await ResetDbAsync();
        var user = await SeedUserAndSessionAsync();
        var client = CreateClient(true);

        var taskList = await TaskListFixture.CreateAsync(user.Pid);

        var dto = new TaskItemCreateDto
        {
            Description = ""
        };

        var response =
            await client.PostAsJsonAsync(
                $"/api/task-lists/{taskList.Pid}/items", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error =
            await response.Content
                .ReadFromJsonAsync<ValidationErrorResponseDto>();
        Assert.NotNull(error);
        Assert.Equal("One or more validation errors occurred.", error.Title);
        Assert.True(error.Errors.ContainsKey("Description"));

        var descriptionErrors = error.Errors["Description"];

        Assert.Contains("Description is required.", descriptionErrors);
        Assert.Contains("Description must be between 3 and 255 characters.",
            descriptionErrors);
    }
}