namespace TodoList.API.DTOs;

public class AuthResponseDto
{
    public required UserResponseDto User { get; init; }
    public required string Token { get; init; }
}