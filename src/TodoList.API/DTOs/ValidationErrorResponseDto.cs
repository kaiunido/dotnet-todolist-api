namespace TodoList.API.DTOs;

public class ValidationErrorResponseDto
{
    public required string Title { get; init; }
    public required IDictionary<string, string[]> Errors { get; init; }
}