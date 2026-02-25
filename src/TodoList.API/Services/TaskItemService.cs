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
        var (_, taskListId) =
            await CheckTaskListOwnerAsync(userPid, taskListPid);

        var query = context.TaskItems
            .Where(ti => ti.TaskListId == taskListId)
            .OrderByDescending(ti => ti.CreatedAt)
            .ThenByDescending(ti => ti.Id);

        page = page < 1 ? 1 : page;
        perPage = perPage < 1 ? 10 : perPage;
        var totalItems = await query.CountAsync();
        var totalPages =
            Math.Max(1, (int)Math.Ceiling((double)totalItems / perPage));

        var items = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(i => new TaskItemResponseDto
            {
                Pid = i.Pid,
                TaskListPid = taskListPid,
                Description = i.Description,
                IsDone = i.IsDone,
                DoneAt = i.DoneAt,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .AsNoTracking()
            .ToListAsync();

        return new PaginationResponse<TaskItemResponseDto>
        {
            Data = items,
            Meta = new PaginationMeta
            {
                Page = page,
                PerPage = perPage,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Links = new PaginationMetaLinks
                {
                    Self = Link(taskListPid, page, perPage),
                    Next = page < totalPages
                        ? Link(taskListPid, page + 1, perPage)
                        : null,
                    Prev = page > 1
                        ? Link(taskListPid, page - 1, perPage)
                        : null
                }
            }
        };
    }

    public async Task<TaskItemResponseDto> GetByPidAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid
    )
    {
        var (_, taskListId) =
            await CheckTaskListOwnerAsync(userPid, taskListPid);

        var taskItem = await context.TaskItems
            .Where(ti => ti.TaskListId == taskListId)
            .Where(ti => ti.Pid == taskItemPid)
            .Select(ti => new TaskItemResponseDto
            {
                Pid = ti.Pid,
                TaskListPid = taskListPid,
                Description = ti.Description,
                IsDone = ti.IsDone,
                DoneAt = ti.DoneAt,
                CreatedAt = ti.CreatedAt,
                UpdatedAt = ti.UpdatedAt
            })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (taskItem is null)
        {
            throw new NotFoundException("Task item not found.");
        }

        return taskItem;
    }

    public async Task<TaskItemResponseDto> CreateAsync(
        Guid userPid,
        Guid taskListPid,
        TaskItemUpsertDto taskItemUpsertDto
    )
    {
        var (_, taskListId) =
            await CheckTaskListOwnerAsync(userPid, taskListPid);

        var taskItem = new TaskItem(
            taskListId,
            taskItemUpsertDto.Description,
            taskItemUpsertDto.IsDone ?? false
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
        TaskItemUpsertDto taskItemUpdateDto
    )
    {
        var (_, taskListId) =
            await CheckTaskListOwnerAsync(userPid, taskListPid);

        var taskItem = await context.TaskItems
            .Where(ti => ti.Pid == taskItemPid)
            .Where(ti => ti.TaskListId == taskListId)
            .SingleOrDefaultAsync();

        if (taskItem is null)
        {
            throw new NotFoundException("Task item not found.");
        }

        taskItem.UpdateDescription(taskItemUpdateDto.Description);

        if (taskItemUpdateDto.IsDone is bool isDone)
        {
            if (isDone)
            {
                taskItem.MarkAsDone();
            }
            else
            {
                taskItem.UnmarkAsDone();
            }
        }

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

    public async Task DeleteAsync(
        Guid userPid,
        Guid taskListPid,
        Guid taskItemPid
    )
    {
        var (_, taskListId) =
            await CheckTaskListOwnerAsync(userPid, taskListPid);

        var taskItem = await context.TaskItems
            .Where(ti => ti.Pid == taskItemPid)
            .Where(ti => ti.TaskListId == taskListId)
            .SingleOrDefaultAsync();

        if (taskItem is null)
        {
            throw new NotFoundException("Task item not found.");
        }

        context.TaskItems.Remove(taskItem);
        await context.SaveChangesAsync();
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

    private static string Link(Guid taskListPid, int page, int perPage)
    {
        return
            $"/api/task-lists/{taskListPid}/items?page={page}&perPage={perPage}";
    }
}