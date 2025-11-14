using Microsoft.EntityFrameworkCore;
using TodoDataAccess.Interfaces;
using TodoDomain.Entities;

namespace TodoDataAccess.Implementation;

public class TodoListRepository : ITodoListRepository
{
    private readonly TodoContext _context;

    public TodoListRepository(TodoContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TodoList>> GetAsync()
    {
        return await _context.TodoList.AsNoTracking().ToListAsync();
    }

    public async Task<TodoList?> GetByIdAsync(long id)
    {
        return await _context.TodoList.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TodoList> AddAsync(TodoList todoList)
    {
        _context.TodoList.Add(todoList);
        await _context.SaveChangesAsync();
        return todoList;
    }

    public async Task<TodoList?> UpdateAsync(long id, string name)
    {
        var todoList = await _context.TodoList.FirstOrDefaultAsync(t => t.Id == id);
        if (todoList == null)
        {
            return null;
        }

        todoList.Name = name;
        await _context.SaveChangesAsync();
        return todoList;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var todoList = await _context.TodoList.FirstOrDefaultAsync(t => t.Id == id);
        if (todoList == null)
        {
            return false;
        }

        _context.TodoList.Remove(todoList);
        await _context.SaveChangesAsync();
        return true;
    }
}
