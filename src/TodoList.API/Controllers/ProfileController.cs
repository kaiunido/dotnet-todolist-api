using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Extensions;
using TodoList.API.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController(
    IUserService userService
) : ControllerBase
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

        if (user == null)
        {
            return NotFound(new
            {
                title = "Not Found", message = "User not found."
            });
        }

        return Ok(user);
    }

    [HttpPatch]
    public async Task<ActionResult<UserResponseDto>> Update(
        [FromBody] UserUpdateDto userUpdateDto)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(
                new
                {
                    title = "Unauthorized", message = "Invalid user identifier."
                }
            );
        }

        var updatedUser =
            await userService.UpdateUserAsync(userPid, userUpdateDto);

        if (updatedUser == null)
        {
            return NotFound(new
            {
                title = "Not Found", message = "User not found."
            });
        }

        return Ok(updatedUser);
    }
}