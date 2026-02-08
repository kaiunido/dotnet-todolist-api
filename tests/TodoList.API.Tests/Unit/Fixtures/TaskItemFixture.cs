using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Tests.Unit.Fixtures;

public class TaskItemFixture
{
    public static async Task<TaskItem> SeedAsync(
        AppDbContext context,
        int taskListId,
        string description,
        bool isDone = false
    )
    {
        var taskItem = new TaskItem(taskListId, description, isDone);

        context.TaskItems.Add(taskItem);
        await context.SaveChangesAsync();

        return taskItem;
    }

    public static async Task<TaskItem> SeedDefaultAsync(
        AppDbContext context,
        int taskListId,
        bool isDone = false
    )
    {
        return await SeedAsync(context, taskListId, "Task Item", isDone);
    }
}