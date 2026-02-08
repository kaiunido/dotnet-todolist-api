using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.DTOs;
using TodoList.API.Exceptions;
using TodoList.API.Models;

namespace TodoList.API.Services;

public class TaskItemService(
    AppDbContext context
) : ITaskItemService
{
    public async Task<PaginationResponse<TaskItemResponseDto>> GetAllAsync(
        Guid userPid,
        Guid taskListPid,
        int page = 1,
        int perPage = 10
    )
    {
        throw new NotImplementedException();
    }

    public async Task<TaskItemResponseDto> GetByPidAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid
    )
    {
        throw new NotImplementedException();
    }

    public async Task<TaskItemResponseDto> CreateAsync(
        Guid userPid,
        Guid taskListPid,
        TaskItemCreateDto taskItemCreateDto
    )
    {
        var (_, taskListId) =
            await CheckTaskListOwnerAsync(userPid, taskListPid);

        var taskItem = new TaskItem(
            taskListId,
            taskItemCreateDto.Description
        );

        context.TaskItems.Add(taskItem);
        await context.SaveChangesAsync();

        return new TaskItemResponseDto
        {
            Pid = taskItem.Pid,
            TaskListPid = taskListPid,
            Description = taskItem.Description,
            IsDone = taskItem.IsDone,
            DoneAt = taskItem.DoneAt,
            CreatedAt = taskItem.CreatedAt,
            UpdatedAt = taskItem.UpdatedAt
        };
    }

    public async Task<TaskItemResponseDto> UpdateAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid,
        TaskItemCreateDto taskItemUpdateDto
    )
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid
    )
    {
        throw new NotImplementedException();
    }

    private async Task<(int UserId, int TaskListId )> CheckTaskListOwnerAsync(
        Guid userPid,
        Guid taskListPid
    )
    {
        var userId = await context.Users
            .Where(u => u.Pid == userPid)
            .Select(u => (int?)u.Id)
            .SingleOrDefaultAsync();

        if (userId is null)
        {
            throw new NotFoundException("User not found.");
        }

        var taskListId = await context.TaskLists
            .Where(tl => tl.Pid == taskListPid)
            .Where(tl => tl.UserId == userId.Value)
            .Select(tl => (int?)tl.Id)
            .SingleOrDefaultAsync();

        if (taskListId is null)
        {
            throw new NotFoundException("Task list not found.");
        }

        return (
            UserId: userId.Value,
            TaskListId: taskListId.Value
        );
    }
}