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