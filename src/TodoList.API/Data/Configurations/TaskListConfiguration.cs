using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.API.Models;

namespace TodoList.API.Data.Configurations;

public class TaskListConfiguration : IEntityTypeConfiguration<TaskList>
{
    public void Configure(EntityTypeBuilder<TaskList> builder)
    {
        builder.HasKey(tl => tl.Id);
        builder.Property(tl => tl.Name).HasMaxLength(100).IsRequired();
        builder.Property(tl => tl.CreatedAt).HasDefaultValueSql("GETDATE()");
        builder.Property(tl => tl.UpdatedAt).HasDefaultValueSql("GETDATE()");
        builder.Property(tl => tl.DeletedAt).HasDefaultValueSql("NULL");

        builder.HasOne(tl => tl.User)
            .WithMany()
            .HasForeignKey(tl => tl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}