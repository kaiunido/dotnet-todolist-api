namespace TodoList.API.Models;

public class TaskList
{
    private TaskList()
    {
        Name = null!;
    }

    public TaskList(int userId, string name)
    {
        UserId = userId;
        Name = name;
    }

    public int Id { get; private set; }
    public Guid Pid { get; private set; } = Guid.NewGuid();

    public int UserId { get; private set; }
    public User? User { get; private set; }

    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public void Rename(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}