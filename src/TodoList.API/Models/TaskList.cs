namespace TodoList.API.Models;

public class TaskList
{
    public int Id { get; init; }
    public Guid Pid { get; init; } = Guid.NewGuid();

    public int UserId { get; init; }
    public User? User { get; init; }

    public required string Name { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; init; } = null;
}