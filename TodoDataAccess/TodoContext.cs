using Microsoft.EntityFrameworkCore;
using TodoDomain.Entities;

namespace TodoDataAccess;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<TodoList> TodoList { get; set; } = default!;
}
