using Microsoft.EntityFrameworkCore;
using TodoDataAccess.Interfaces;
using TodoDomain.Entities;

namespace TodoDataAccess.Implementation;

public class TodoItemRepository : ITodoItemRepository
{
    private readonly TodoContext _context;

    public TodoItemRepository(TodoContext context)
    {
        _context = context;
    }

    public Task<TodoItem?> GetByIdAsync(long todoListId, long todoItemId)
    {
        return _context.TodoItem
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TodoListId == todoListId && t.Id == todoItemId);
    }

    public async Task<TodoItem?> AddAsync(long todoListId, string description)
    {
        var listExists = await _context.TodoList.AsNoTracking().AnyAsync(t => t.Id == todoListId);
        if (!listExists)
        {
            return null;
        }

        var todoItem = new TodoItem
        {
            TodoListId = todoListId,
            Description = description,
            IsCompleted = false
        };

        _context.TodoItem.Add(todoItem);
        await _context.SaveChangesAsync();
        return todoItem;
    }

    public async Task<TodoItem?> UpdateDescriptionAsync(long todoListId, long todoItemId, string description)
    {
        var todoItem = await FindTrackedAsync(todoListId, todoItemId);
        if (todoItem == null)
        {
            return null;
        }

        todoItem.Description = description;
        await _context.SaveChangesAsync();
        return todoItem;
    }

    public async Task<TodoItem?> CompleteAsync(long todoListId, long todoItemId)
    {
        var todoItem = await FindTrackedAsync(todoListId, todoItemId);
        if (todoItem == null)
        {
            return null;
        }

        if (!todoItem.IsCompleted)
        {
            todoItem.IsCompleted = true;
            await _context.SaveChangesAsync();
        }

        return todoItem;
    }

    public async Task<bool> DeleteAsync(long todoListId, long todoItemId)
    {
        var todoItem = await FindTrackedAsync(todoListId, todoItemId);
        if (todoItem == null)
        {
            return false;
        }

        _context.TodoItem.Remove(todoItem);
        await _context.SaveChangesAsync();
        return true;
    }

    private Task<TodoItem?> FindTrackedAsync(long todoListId, long todoItemId)
    {
        return _context.TodoItem.FirstOrDefaultAsync(t =>
            t.TodoListId == todoListId && t.Id == todoItemId);
    }
}
