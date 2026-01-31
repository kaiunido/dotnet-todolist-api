namespace TodoList.API.Models;

public class UserSession
{
    public int Id { get; init; }
    public required Guid Jti { get; init; }
    public int UserId { get; init; }
    public string? DeviceInfo { get; init; }
    public string? IpAddress { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public User User { get; init; } = null!;
}