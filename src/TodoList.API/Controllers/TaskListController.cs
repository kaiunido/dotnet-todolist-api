using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Extensions;
using TodoList.API.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Authorize]
[Route("api/task-lists")]
public class TaskListController(
    ITaskListService taskListService
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<TaskListResponseDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1, [FromQuery] int perPage = 10
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
            await taskListService.GetAllAsync(userPid, page, perPage);

        return Ok(response);
    }

    [HttpGet("{pid:guid}")]
    [ProducesResponseType(typeof(TaskListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPid(Guid pid)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new ErrorResponseDto
            {
                Title = "Unauthorized", Message = "Invalid user identifier."
            });
        }

        var response =
            await taskListService.GetByPidAsync(userPid, pid);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskListResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponseDto),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] TaskListCreateDto createTaskListDto)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new ErrorResponseDto
            {
                Title = "Unauthorized", Message = "Invalid user identifier."
            });
        }

        var response =
            await taskListService.CreateAsync(userPid, createTaskListDto);

        return CreatedAtAction(
            "GetByPid",
            new { pid = response.Pid },
            response
        );
    }

    [HttpPatch("{pid:guid}")]
    [ProducesResponseType(typeof(TaskListResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponseDto),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid pid,
        [FromBody] TaskListUpdateDto updateTaskListDto
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
            await taskListService.UpdateAsync(userPid, pid,
                updateTaskListDto);

        return Ok(response);
    }

    [HttpDelete("{pid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid pid)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new ErrorResponseDto
            {
                Title = "Unauthorized", Message = "Invalid user identifier."
            });
        }

        await taskListService.DeleteAsync(userPid, pid);

        return NoContent();
    }
}