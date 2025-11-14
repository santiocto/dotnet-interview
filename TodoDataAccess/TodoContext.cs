using Microsoft.EntityFrameworkCore;
using TodoDomain.Entities;

namespace TodoDataAccess;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<TodoList> TodoList { get; set; } = null!;
    public DbSet<TodoItem> TodoItem { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>()
            .Property(t => t.Description)
            .IsRequired();

        modelBuilder.Entity<TodoItem>()
            .HasOne(t => t.TodoList)
            .WithMany(l => l.Items)
            .HasForeignKey(t => t.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
