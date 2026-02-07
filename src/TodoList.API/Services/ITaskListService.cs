using TodoList.API.DTOs;

namespace TodoList.API.Services;

public interface ITaskListService
{
    Task<TaskListResponseDto> GetByPidAsync(Guid userPid, Guid taskListPid);

    Task<TaskListResponseDto> CreateAsync(
        Guid userPid,
        TaskListCreateDto taskListCreateDto
    );
}