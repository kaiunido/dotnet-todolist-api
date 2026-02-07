namespace TodoList.API.DTOs;

public class PaginationMeta
{
    public int Page { get; init; }
    public int PerPage { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
    public PaginationMetaLinks Links { get; init; } = new();
}