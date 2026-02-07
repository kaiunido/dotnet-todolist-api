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
    public async Task<IActionResult> Get()
    {
        return Ok();
    }

    [HttpGet("{pid:guid}")]
    public async Task<IActionResult> GetByPid(Guid pid)
    {
        return Ok();
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

    [HttpPatch]
    public async Task<IActionResult> Update(Guid taskListPid,
        [FromBody] TaskListUpdateDto updateTaskListDto)
    {
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid taskListPid)
    {
        return NoContent();
    }
}