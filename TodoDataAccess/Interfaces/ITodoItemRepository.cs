using TodoDomain.Entities;

namespace TodoDataAccess.Interfaces;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetByIdAsync(long todoListId, long todoItemId);
    Task<TodoItem?> AddAsync(long todoListId, string description);
    Task<TodoItem?> UpdateDescriptionAsync(long todoListId, long todoItemId, string description);
    Task<TodoItem?> CompleteAsync(long todoListId, long todoItemId);
    Task<bool> DeleteAsync(long todoListId, long todoItemId);
}
