using TodoList.API.DTOs;

namespace TodoList.API.Services;

public interface ITaskListService
{
    Task<PaginationResponse<TaskListResponseDto>> GetAllAsync(Guid userPid,
        int page = 1, int perPage = 10);

    Task<TaskListResponseDto> GetByPidAsync(Guid userPid, Guid taskListPid);

    Task<TaskListResponseDto> CreateAsync(
        Guid userPid,
        TaskListCreateDto taskListCreateDto
    );

    Task<TaskListResponseDto> UpdateAsync(
        Guid userPid,
        Guid taskListPid,
        TaskListUpdateDto updateTaskListDto
    );

    Task DeleteAsync(Guid userPid, Guid taskListPid);
}