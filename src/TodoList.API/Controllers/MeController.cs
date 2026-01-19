using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Extensions;
using TodoList.API.Services;

namespace TodoList.API.Controllers;

[Authorize]
[ApiController]
[Route("api/me")]
public class MeController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserResponseDto>> Get()
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new
            {
                title = "Unauthorized", message = "Invalid user identifier."
            });
        }

        var user = await userService.GetUserByPidAsync(userPid);

        if (user is null)
        {
            return NotFound(new
            {
                title = "Not Found",
                message = "User not found."
            });
        }

        return Ok(user);
    }
}