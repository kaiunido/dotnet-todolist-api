namespace TodoList.API.DTOs;

public class TaskListResponseDto
{
    public Guid Pid { get; init; }
    public required string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}