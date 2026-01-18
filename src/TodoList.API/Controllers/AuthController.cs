using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.API.Services;

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

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var jtiClaim = User
            .FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames
                .Jti)?.Value;

        if (string.IsNullOrEmpty(jtiClaim)) return BadRequest();

        await _authService.LogoutAsync(Guid.Parse(jtiClaim));

        return NoContent();
    }
}