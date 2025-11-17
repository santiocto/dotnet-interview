using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
public static class TodoTools
{
    [McpServerTool(Name = "get_todo_lists")]
    [Description("Obtiene todas las listas de TODO.")]
    public static Task<string> GetTodoListsAsync(
        TodoApiClient api,
        CancellationToken ct)
        => api.GetTodoListsRawAsync(ct);

    [McpServerTool(Name = "get_todo_list_by_id")]
    [Description("Obtiene una lista de TODO por id. Devuelve null si no existe.")]
    public static Task<string?> GetTodoListByIdAsync(
        TodoApiClient api,
        long id,
        CancellationToken ct)
        => api.GetTodoListRawAsync(id, ct);

    [McpServerTool(Name = "create_todo_list")]
    [Description("Crea una nueva lista de TODO con el nombre indicado.")]
    public static Task<string> CreateTodoListAsync(
        TodoApiClient api,
        string name,
        CancellationToken ct)
        => api.CreateTodoListRawAsync(name, ct);

    [McpServerTool(Name = "update_todo_list")]
    [Description("Actualiza el nombre de una lista de TODO. Devuelve null si no existe.")]
    public static Task<string?> UpdateTodoListAsync(
        TodoApiClient api,
        long id,
        string name,
        CancellationToken ct)
        => api.UpdateTodoListRawAsync(id, name, ct);

    [McpServerTool(Name = "delete_todo_list")]
    [Description("Elimina una lista de TODO. Devuelve true si se borró, false si no existe.")]
    public static Task<bool> DeleteTodoListAsync(
        TodoApiClient api,
        long id,
        CancellationToken ct)
        => api.DeleteTodoListAsync(id, ct);

    [McpServerTool(Name = "get_todo_item")]
    [Description("Obtiene un item por ids de lista y item. Devuelve null si no existe.")]
    public static Task<string?> GetTodoItemAsync(
        TodoApiClient api,
        long todoListId,
        long itemId,
        CancellationToken ct)
        => api.GetTodoItemRawAsync(todoListId, itemId, ct);

    [McpServerTool(Name = "create_todo_item")]
    [Description("Crea un item en la lista indicada con la descripción proporcionada.")]
    public static Task<string?> CreateTodoItemAsync(
        TodoApiClient api,
        long todoListId,
        string description,
        CancellationToken ct)
        => api.CreateTodoItemRawAsync(todoListId, description, ct);

    [McpServerTool(Name = "update_todo_item")]
    [Description("Actualiza la descripción de un item existente. Devuelve null si no existe.")]
    public static Task<string?> UpdateTodoItemAsync(
        TodoApiClient api,
        long todoListId,
        long itemId,
        string description,
        CancellationToken ct)
        => api.UpdateTodoItemRawAsync(todoListId, itemId, description, ct);

    [McpServerTool(Name = "complete_todo_item")]
    [Description("Marca como completo un item existente. Devuelve null si no existe.")]
    public static Task<string?> CompleteTodoItemAsync(
        TodoApiClient api,
        long todoListId,
        long itemId,
        CancellationToken ct)
        => api.CompleteTodoItemRawAsync(todoListId, itemId, ct);

    [McpServerTool(Name = "delete_todo_item")]
    [Description("Elimina un item en la lista indicada. Devuelve true si se borró, false si no existe.")]
    public static Task<bool> DeleteTodoItemAsync(
        TodoApiClient api,
        long todoListId,
        long itemId,
        CancellationToken ct)
        => api.DeleteTodoItemAsync(todoListId, itemId, ct);
}
