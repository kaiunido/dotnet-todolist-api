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
            .Where(tl => tl.UserId == userId)
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
            .Select(u => u.Id)
            .FirstOrDefaultAsync();

        if (userId == 0)
        {
            throw new NotFoundException("User not found.");
        }

        var taskList = new TaskList
        {
            UserId = userId,
            Name = taskListCreateDto.Name
        };

        context.TaskLists.Add(taskList);
        await context.SaveChangesAsync();

        return new TaskListResponseDto
        {
            Pid = taskList.Pid,
            Name = taskList.Name
        };
    }
}