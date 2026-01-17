using Microsoft.EntityFrameworkCore;
using TodoList.API.Models;

namespace TodoList.API.Data;

public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext)
            .Assembly);
    }
}