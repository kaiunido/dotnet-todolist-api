using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.Models;

namespace TodoList.API.Tests.Integration.Fixtures;

public class TaskListFixture(
    Func<Func<AppDbContext, Task>, Task> withDb
)
{
    public async Task<TaskList> CreateAsync(Guid userPid,
        string name = "Test List")
    {
        TaskList? entity = null;

        await withDb(async db =>
        {
            var userId = await db.Users
                .Where(u => u.Pid == userPid)
                .Select(u => u.Id)
                .SingleAsync();

            entity = new TaskList
            {
                Pid = Guid.NewGuid(),
                Name = name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.TaskLists.Add(entity);
        });

        return entity ??
               throw new InvalidOperationException(
                   "TaskListFixture failed to create entity.");
    }
}