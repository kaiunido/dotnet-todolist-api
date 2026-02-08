using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Tests.Unit.Fixtures;

public class TaskListFixture
{
    public static async Task<TaskList> SeedAsync(
        AppDbContext context,
        int userId,
        string name
    )
    {
        var taskList = new TaskList(userId, name);

        context.TaskLists.Add(taskList);
        await context.SaveChangesAsync();

        return taskList;
    }

    public static async Task<TaskList> SeedDefaultAsync(
        AppDbContext context,
        int userId
    )
    {
        return await SeedAsync(context, userId, "My default task list");
    }
}