using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TodoList.API.Extensions;
using TodoList.API.Models;

namespace TodoList.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserSession> UserSessions { get; set; } = null!;
    public DbSet<TaskList> TaskLists { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext)
            .Assembly);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Table name
            var tableName = entity.GetTableName();
            if (tableName is null)
            {
                continue;
            }

            entity.SetTableName(tableName.ToSnakeCase());

            // Store object (table + schema)
            var storeObject = StoreObjectIdentifier.Table(
                entity.GetTableName()!,
                entity.GetSchema()
            );

            // Columns
            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName(storeObject);
                if (columnName is not null)
                {
                    property.SetColumnName(columnName.ToSnakeCase());
                }
            }

            // Keys
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (keyName is not null)
                {
                    key.SetName(keyName.ToSnakeCase());
                }
            }

            // Foreign keys
            foreach (var fk in entity.GetForeignKeys())
            {
                var fkName = fk.GetConstraintName();
                if (fkName is not null)
                {
                    fk.SetConstraintName(fkName.ToSnakeCase());
                }
            }

            // Indexes
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (indexName is not null)
                {
                    index.SetDatabaseName(indexName.ToSnakeCase());
                }
            }
        }
    }
}