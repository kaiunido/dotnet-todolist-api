using TodoList.API.DTOs;

namespace TodoList.API.Services;

public interface ITaskListService
{
    Task<TaskListResponseDto> CreateAsync(
        Guid userPid,
        TaskListCreateDto taskListCreateDto
    );
}