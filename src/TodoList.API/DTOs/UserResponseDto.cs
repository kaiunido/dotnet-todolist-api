namespace TodoList.API.DTOs;

public class UserResponseDto
{
    public Guid Pid { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public DateTime CreatedAt { get; init; }
}