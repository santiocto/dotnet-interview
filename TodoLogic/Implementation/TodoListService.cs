using TodoDataAccess.Interfaces;
using TodoDomain.Entities;
using TodoLogic.Interfaces;

namespace TodoLogic.Implementation;

public class TodoListService : ITodoListService
{
    private readonly ITodoListRepository _repository;

    public TodoListService(ITodoListRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<TodoList>> GetAsync()
    {
        return _repository.GetAsync();
    }

    public Task<TodoList?> GetByIdAsync(long id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<TodoList> CreateAsync(string name)
    {
        var todoList = new TodoList { Name = name };
        return _repository.AddAsync(todoList);
    }

    public Task<TodoList?> UpdateAsync(long id, string name)
    {
        return _repository.UpdateAsync(id, name);
    }

    public Task<bool> DeleteAsync(long id)
    {
        return _repository.DeleteAsync(id);
    }
}
