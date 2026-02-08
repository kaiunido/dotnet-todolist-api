using TodoList.API.DTOs;

namespace TodoList.API.Services;

public interface ITaskItemService
{
    Task<PaginationResponse<TaskItemResponseDto>> GetAllAsync(
        Guid userPid,
        Guid taskListPid,
        int page = 1,
        int perPage = 10
    );

    Task<TaskItemResponseDto> GetByPidAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid
    );

    Task<TaskItemResponseDto> CreateAsync(
        Guid userPid,
        Guid taskListPid,
        TaskItemCreateDto taskItemCreateDto
    );

    Task<TaskItemResponseDto> UpdateAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid,
        TaskItemCreateDto taskItemUpdateDto
    );

    Task DeleteAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid
    );
}