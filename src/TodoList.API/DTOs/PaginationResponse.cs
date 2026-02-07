namespace TodoList.API.DTOs;

public class PaginationResponse<T>
{
    public IReadOnlyList<T> Data { get; init; } = [];
    public PaginationMeta Meta { get; init; } = new();
}