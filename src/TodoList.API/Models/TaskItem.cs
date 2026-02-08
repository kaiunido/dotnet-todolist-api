namespace TodoList.API.Models;

public class TaskItem
{
    private TaskItem()
    {
        Description = null!;
    }

    public TaskItem(
        int taskListId,
        string description,
        bool isDone = false
    )
    {
        TaskListId = taskListId;
        Description = GetNormalizedDescription(description);

        if (isDone)
        {
            MarkAsDone();
        }
    }

    public int Id { get; private set; }
    public Guid Pid { get; private set; } = Guid.NewGuid();
    public int TaskListId { get; private set; }
    public TaskList? TaskList { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsDone { get; private set; }
    public DateTime? DoneAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public void MarkAsDone()
    {
        if (IsDone)
        {
            return;
        }

        IsDone = true;
        DoneAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnmarkAsDone()
    {
        if (!IsDone)
        {
            return;
        }

        IsDone = false;
        DoneAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string newDescription)
    {
        var normalizedDescription = GetNormalizedDescription(newDescription);

        if (normalizedDescription == Description)
        {
            return;
        }

        Description = normalizedDescription;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GetNormalizedDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty.",
                nameof(description));
        }

        return description.Trim();
    }
}