using TodoDomain.Entities;

namespace TodoDataAccess.Interfaces;

public interface ITodoListRepository
{
    Task<IReadOnlyList<TodoList>> GetAsync();
    Task<TodoList?> GetByIdAsync(long id);
    Task<TodoList> AddAsync(TodoList todoList);
    Task<TodoList?> UpdateAsync(long id, string name);
    Task<bool> DeleteAsync(long id);
}
