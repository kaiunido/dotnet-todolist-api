using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.DTOs;
using TodoList.API.Exceptions;
using TodoList.API.Models;

namespace TodoList.API.Services;

public class TaskListService(
    AppDbContext context
) : ITaskListService
{
    public async Task<TaskListResponseDto> GetByPidAsync(Guid userPid,
        Guid taskListPid)
    {
        var userId = await context.Users
            .Where(u => u.Pid == userPid)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        if (userId is null)
        {
            throw new NotFoundException("User not found.");
        }

        var taskList = await context.TaskLists
            .Where(tl => tl.Pid == taskListPid)
            .Where(tl => tl.UserId == userId.Value)
            .FirstOrDefaultAsync();

        if (taskList is null)
        {
            throw new NotFoundException("Task list not found.");
        }

        return new TaskListResponseDto
        {
            Pid = taskList.Pid,
            Name = taskList.Name,
            CreatedAt = taskList.CreatedAt,
            UpdatedAt = taskList.UpdatedAt
        };
    }

    public async Task<TaskListResponseDto> CreateAsync(
        Guid userPid,
        TaskListCreateDto taskListCreateDto
    )
    {
        var userId = await context.Users
            .Where(u => u.Pid == userPid)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        if (userId is null)
        {
            throw new NotFoundException("User not found.");
        }

        var taskList = new TaskList(userId.Value, taskListCreateDto.Name);

        context.TaskLists.Add(taskList);
        await context.SaveChangesAsync();

        return new TaskListResponseDto
        {
            Pid = taskList.Pid,
            Name = taskList.Name,
            CreatedAt = taskList.CreatedAt,
            UpdatedAt = taskList.UpdatedAt
        };
    }

    public async Task<TaskListResponseDto> UpdateAsync(
        Guid userPid,
        Guid taskListPid,
        TaskListUpdateDto updateTaskListDto
    )
    {
        var userId =
            await context.Users
                .Where(u => u.Pid == userPid)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync();

        if (userId is null)
        {
            throw new NotFoundException("User not found.");
        }

        var taskList = await context.TaskLists
            .SingleOrDefaultAsync(tl =>
                tl.Pid == taskListPid &&
                tl.UserId == userId.Value
            );

        if (taskList is null)
        {
            throw new NotFoundException("Task list not found.");
        }

        taskList.Rename(updateTaskListDto.Name);
        await context.SaveChangesAsync();

        return new TaskListResponseDto
        {
            Pid = taskList.Pid,
            Name = taskList.Name,
            CreatedAt = taskList.CreatedAt,
            UpdatedAt = taskList.UpdatedAt
        };
    }

    public async Task<PaginationResponse<TaskListResponseDto>> GetAllAsync(
        Guid userPid, int page = 1, int perPage = 10)
    {
        var userId = await context.Users
            .Where(u => u.Pid == userPid)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        if (userId is null)
        {
            throw new NotFoundException("User not found.");
        }

        page = page < 1 ? 1 : page;
        perPage = perPage < 1 ? 10 : perPage;
        perPage = Math.Min(perPage, 100);

        var totalTaskLists = await context.TaskLists
            .CountAsync(tl => tl.UserId == userId.Value);

        var totalPages = Math.Max(1,
            (int)Math.Ceiling(totalTaskLists / (double)perPage));
        page = Math.Min(page, totalPages);

        var taskLists = totalTaskLists == 0
            ? []
            : await context.TaskLists
                .Where(tl => tl.UserId == userId.Value)
                .OrderByDescending(tl => tl.CreatedAt)
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(tl => new TaskListResponseDto
                {
                    Pid = tl.Pid,
                    Name = tl.Name,
                    CreatedAt = tl.CreatedAt,
                    UpdatedAt = tl.UpdatedAt
                })
                .ToListAsync();

        return new PaginationResponse<TaskListResponseDto>
        {
            Data = taskLists,
            Meta = new PaginationMeta
            {
                Page = page,
                PerPage = perPage,
                TotalItems = totalTaskLists,
                TotalPages = totalPages,
                Links = new PaginationMetaLinks
                {
                    Self = Link(page, perPage),
                    Next = page < totalPages
                        ? Link(page + 1, perPage)
                        : null,
                    Prev = page > 1
                        ? Link(page - 1, perPage)
                        : null
                }
            }
        };
    }

    public async Task DeleteAsync(Guid userPid, Guid taskListPid)
    {
        var userId = await context.Users
            .Where(u => u.Pid == userPid)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        if (userId is null)
        {
            throw new NotFoundException("User not found.");
        }

        var taskList = await context.TaskLists
            .SingleOrDefaultAsync(tl =>
                tl.Pid == taskListPid && tl.UserId == userId.Value);

        if (taskList is null)
        {
            throw new NotFoundException("Task list not found.");
        }

        context.TaskLists.Remove(taskList);
        await context.SaveChangesAsync();
    }

    private static string Link(int page, int perPage)
    {
        return $"/api/task-lists?page={page}&perPage={perPage}";
    }
}