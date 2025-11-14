using TodoDomain.Entities;

namespace TodoLogic.Interfaces;

public interface ITodoListService
{
    Task<IReadOnlyList<TodoList>> GetAsync();
    Task<TodoList?> GetByIdAsync(long id);
    Task<TodoList> CreateAsync(string name);
    Task<TodoList?> UpdateAsync(long id, string name);
    Task<bool> DeleteAsync(long id);
}
