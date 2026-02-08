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
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1, [FromQuery] int perPage = 10
    )
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new
            {
                title = "Unauthorized", message = "Invalid user identifier."
            });
        }

        var response =
            await taskListService.GetAllAsync(userPid, page, perPage);

        return Ok(response);
    }

    [HttpGet("{pid:guid}")]
    public async Task<IActionResult> GetByPid(Guid pid)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new
            {
                title = "Unauthorized", message = "Invalid user identifier."
            });
        }

        var response =
            await taskListService.GetByPidAsync(userPid, pid);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TaskListCreateDto createTaskListDto)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new
            {
                title = "Unauthorized", message = "Invalid user identifier."
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
    public async Task<IActionResult> Update(
        Guid pid,
        [FromBody] TaskListUpdateDto updateTaskListDto
    )
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new
            {
                title = "Unauthorized", message = "Invalid user identifier."
            });
        }

        var response =
            await taskListService.UpdateAsync(userPid, pid,
                updateTaskListDto);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid taskListPid)
    {
        return NoContent();
    }
}