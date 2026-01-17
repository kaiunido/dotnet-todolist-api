namespace TodoList.API.Models;

public class User
{
    public int Id { get; init; }
    public Guid Pid { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}