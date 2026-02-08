namespace TodoList.API.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, string? title = null)
        : base(message)
    {
        Title = title ?? "Not Found";
    }

    public string? Title { get; }
}