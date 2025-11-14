using TodoDomain.Entities;

namespace TodoLogic.Interfaces;

public interface ITodoItemService
{
    Task<TodoItem?> GetByIdAsync(long todoListId, long todoItemId);
    Task<TodoItem?> CreateAsync(long todoListId, string description);
    Task<TodoItem?> UpdateDescriptionAsync(long todoListId, long todoItemId, string description);
    Task<TodoItem?> CompleteAsync(long todoListId, long todoItemId);
    Task<bool> DeleteAsync(long todoListId, long todoItemId);
}
