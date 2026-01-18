namespace TodoList.API.DTOs;

public class UserResponseDto
{
    public Guid Pid { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public DateTime CreatedAt { get; init; }
}