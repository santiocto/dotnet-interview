using TodoDataAccess.Interfaces;
using TodoDomain.Entities;
using TodoLogic.Interfaces;

namespace TodoLogic.Implementation;

public class TodoItemService : ITodoItemService
{
    private readonly ITodoItemRepository _repository;

    public TodoItemService(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public Task<TodoItem?> GetByIdAsync(long todoListId, long todoItemId)
    {
        return _repository.GetByIdAsync(todoListId, todoItemId);
    }

    public Task<TodoItem?> CreateAsync(long todoListId, string description)
    {
        return _repository.AddAsync(todoListId, description);
    }

    public Task<TodoItem?> UpdateDescriptionAsync(long todoListId, long todoItemId, string description)
    {
        return _repository.UpdateDescriptionAsync(todoListId, todoItemId, description);
    }

    public Task<TodoItem?> CompleteAsync(long todoListId, long todoItemId)
    {
        return _repository.CompleteAsync(todoListId, todoItemId);
    }

    public Task<bool> DeleteAsync(long todoListId, long todoItemId)
    {
        return _repository.DeleteAsync(todoListId, todoItemId);
    }
}
