namespace TodoList.API.DTOs;

public class TaskItemCreateDto
{
    public required string Description { get; init; } = string.Empty;
    public bool? IsDone { get; init; }
}