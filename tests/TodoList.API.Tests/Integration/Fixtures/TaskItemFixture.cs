using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Tests.Integration.Fixtures;

public class TaskItemFixture(
    Func<Func<AppDbContext, Task>, Task> withDb
)
{
    public async Task<TaskItem> CreateAsync(Guid TaskListPid,
        string description = "Test item description")
    {
        TaskItem? entity = null;

        await withDb(async db =>
        {
            var taskListId = await db.TaskLists
                .Where(tl => tl.Pid == TaskListPid)
                .AsNoTracking()
                .Select(tl => tl.Id)
                .SingleAsync();

            entity = new TaskItem(taskListId, description);

            db.TaskItems.Add(entity);
        });

        return entity ??
               throw new InvalidOperationException(
                   "TaskItemFixture failed to create entity.");
    }
}