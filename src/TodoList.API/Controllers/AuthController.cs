using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Services;
using TodoList.API.Validators;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var loginResponse = await _authService.LoginAsync(loginDto);

        return Ok(loginResponse);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserRegisterDto userRegisterDto)
    {
        var authResponse = await _authService.RegisterAsync(userRegisterDto);

        return CreatedAtAction(
            "GetById",
            "Users",
            new {pid = authResponse.User.Pid},
            authResponse
        );
    }
}