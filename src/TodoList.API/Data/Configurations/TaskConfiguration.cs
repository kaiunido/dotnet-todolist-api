using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.API.Models;

namespace TodoList.API.Data.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Pid)
            .IsRequired();

        builder.HasIndex(t => t.Pid)
            .IsUnique();

        builder.Property(t => t.TaskListId)
            .IsRequired();

        builder.HasOne(t => t.TaskList)
            .WithMany()
            .HasForeignKey(t => t.TaskListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.Description)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.IsDone)
            .IsRequired();

        builder.Property(t => t.DoneAt)
            .IsRequired(false);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.HasIndex(t => t.TaskListId);
        builder.HasIndex(t => new
        {
            t.TaskListId, t.IsDone
        });
    }
}