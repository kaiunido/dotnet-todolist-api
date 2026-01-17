using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUserService _userService
) : ControllerBase {
    [HttpGet("{pid:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid pid)
    {
        var user = await _userService.GetUserByIdAsync(pid);

        if (user == null) return NotFound();

        return Ok(user);
    }
}