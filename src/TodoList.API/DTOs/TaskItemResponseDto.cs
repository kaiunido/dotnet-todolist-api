namespace TodoList.API.DTOs;

public class TaskItemResponseDto
{
    public Guid Pid { get; init; }
    public Guid TaskListPid { get; init; }
    public required string Description { get; init; }
    public bool IsDone { get; init; }
    public DateTime? DoneAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}