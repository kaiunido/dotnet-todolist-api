using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Extensions;
using TodoList.API.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Authorize]
[Route("api/task-lists/{taskListPid:guid}/items")]
public class TaskItemController(
    ITaskItemService taskItemService
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        Guid taskListPid,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10
    )
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new ErrorResponseDto
            {
                Title = "Unauthorized", Message = "Invalid user identifier."
            });
        }

        var response =
            await taskItemService.GetAllAsync(userPid, taskListPid, page,
                perPage);

        return Ok(response);
    }

    [HttpGet("{pid:guid}")]
    public async Task<IActionResult> GetByPid(Guid taskListPid, Guid pid)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new ErrorResponseDto
            {
                Title = "Unauthorized", Message = "Invalid user identifier."
            });
        }

        var response =
            await taskItemService.GetByPidAsync(userPid, taskListPid, pid);

        return Ok(response);
    }
}