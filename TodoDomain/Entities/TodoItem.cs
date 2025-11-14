namespace TodoDomain.Entities;

public class TodoItem
{
    public long Id { get; set; }
    public long TodoListId { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
    public TodoList? TodoList { get; set; }
}
