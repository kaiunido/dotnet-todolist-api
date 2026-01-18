using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(
    IUserService userService
) : ControllerBase {
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseDto>> LoggedInUser()
    {
        var pidClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(pidClaim, out var userPid))
        {
            return Unauthorized(
                new { title = "Unauthorized", message = "Invalid user identifier." }
                );
        }

        var user = await userService.GetUserByPidAsync(userPid);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{pid:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid pid)
    {
        var user = await userService.GetUserByPidAsync(pid);

        if (user == null) return NotFound();

        return Ok(user);
    }
}