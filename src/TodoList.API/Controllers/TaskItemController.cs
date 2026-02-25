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
    public async Task<ActionResult<PaginationResponse<TaskItemResponseDto>>>
        Get(
            [FromRoute] Guid taskListPid,
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
    public async Task<ActionResult<TaskItemResponseDto>> GetByPid(
        [FromRoute] Guid taskListPid,
        [FromRoute] Guid pid
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
            await taskItemService.GetByPidAsync(userPid, taskListPid, pid);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemResponseDto>> Create(
        [FromRoute] Guid taskListPid,
        [FromBody] TaskItemCreateDto itemDto
    )
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new ErrorResponseDto
            {
                Title = "Unauthorized", Message = "Invalid user identifier."
            });
        }

        var response = await taskItemService.CreateAsync(userPid, taskListPid,
            itemDto);

        return CreatedAtAction(
            nameof(GetByPid),
            new { taskListPid, pid = response.Pid },
            response
        );
    }
}