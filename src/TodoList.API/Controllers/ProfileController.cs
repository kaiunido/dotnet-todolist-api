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

    [HttpPatch("password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        if (!User.TryGetUserPid(out var userPid))
        {
            return Unauthorized(new
            {
                title = "Unauthorized", message = "Invalid user identifier."
            });
        }

        var deviceInfo = Request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(deviceInfo))
        {
            deviceInfo = "Unknown Device";
        }

        var response = await userService.ChangePasswordAsync(
            userPid,
            changePasswordDto,
            deviceInfo
        );

        if (response is null)
        {
            return NotFound(new
            {
                title = "Not Found", message = "User not found."
            });
        }

        return Ok(response);
    }
}