namespace TodoList.API.DTOs;

public class ErrorResponseDto
{
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}