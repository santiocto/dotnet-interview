using Microsoft.AspNetCore.Mvc;
using TodoDomain.Dtos;
using TodoDomain.Entities;
using TodoLogic.Interfaces;

namespace TodoApi.Controllers
{
    [Route("api/todolists")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly ITodoListService _todoListService;

        public TodoListsController(ITodoListService todoListService)
        {
            _todoListService = todoListService;
        }

        // GET: api/todolists
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TodoList>>> GetTodoLists()
        {
            var todoLists = await _todoListService.GetAsync();
            return Ok(todoLists);
        }

        // GET: api/todolists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoList>> GetTodoList(long id)
        {
            var todoList = await _todoListService.GetByIdAsync(id);
            return Ok(todoList);
        }

        // PUT: api/todolists/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload)
        {
            var updated = await _todoListService.UpdateAsync(id, payload.Name);
            return Ok(updated);
        }

        // POST: api/todolists
        [HttpPost]
        public async Task<ActionResult<TodoList>> PostTodoList(CreateTodoList payload)
        {
            var todoList = await _todoListService.CreateAsync(payload.Name);
            return CreatedAtAction(nameof(GetTodoList), new { id = todoList.Id }, todoList);
        }

        // DELETE: api/todolists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoList(long id)
        {
            var deleted = await _todoListService.DeleteAsync(id);
            return NoContent();
        }
    }
}
