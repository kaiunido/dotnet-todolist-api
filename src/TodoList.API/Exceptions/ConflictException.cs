namespace TodoList.API.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message, string? title = null)
        : base(message)
    {
        Title = title ?? "Conflict";
    }

    public string? Title { get; }
}