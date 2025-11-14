using Microsoft.AspNetCore.Mvc;
using TodoDomain.Dtos;
using TodoDomain.Entities;
using TodoLogic.Interfaces;

namespace TodoApi.Controllers
{
    [Route("api/todolists/{todoListId}/items")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;

        public TodoItemsController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        [HttpGet("{itemId}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long todoListId, long itemId)
        {
            var todoItem = await _todoItemService.GetByIdAsync(todoListId, itemId);
            return todoItem is null ? NotFound() : Ok(todoItem);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(long todoListId, CreateTodoItem payload)
        {
            var todoItem = await _todoItemService.CreateAsync(todoListId, payload.Description);
            return todoItem is null
                ? NotFound()
                : CreatedAtAction(nameof(GetTodoItem), new { todoListId, itemId = todoItem.Id }, todoItem);
        }

        [HttpPut("{itemId}")]
        public async Task<ActionResult> PutTodoItem(long todoListId, long itemId, UpdateTodoItem payload)
        {
            var todoItem = await _todoItemService.UpdateDescriptionAsync(todoListId, itemId, payload.Description);
            return todoItem is null ? NotFound() : Ok(todoItem);
        }

        [HttpPost("{itemId}/complete")]
        public async Task<ActionResult> CompleteTodoItem(long todoListId, long itemId)
        {
            var todoItem = await _todoItemService.CompleteAsync(todoListId, itemId);
            return todoItem is null ? NotFound() : Ok(todoItem);
        }

        [HttpDelete("{itemId}")]
        public async Task<ActionResult> DeleteTodoItem(long todoListId, long itemId)
        {
            var deleted = await _todoItemService.DeleteAsync(todoListId, itemId);
            return deleted ? NoContent() : NotFound();
        }
    }
}
