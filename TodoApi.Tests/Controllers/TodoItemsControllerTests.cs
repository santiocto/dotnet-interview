using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoDomain.Dtos;
using TodoDomain.Entities;
using TodoLogic.Interfaces;

namespace TodoApiTests.Controllers
{
    [TestClass]
    public class TodoItemsControllerTests
    {
        private Mock<ITodoItemService> _todoItemServiceMock = null!;
        private TodoItemsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _todoItemServiceMock = new Mock<ITodoItemService>(MockBehavior.Strict);
            _controller = new TodoItemsController(_todoItemServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _todoItemServiceMock.VerifyAll();
        }

        [TestMethod]
        public async Task GetTodoItem_ReturnsOk_WhenFound()
        {
            var todoItem = new TodoItem { Id = 1, TodoListId = 5, Description = "Sample" };

            _todoItemServiceMock
                .Setup(s => s.GetByIdAsync(todoItem.TodoListId, todoItem.Id))
                .ReturnsAsync(todoItem);

            var result = await _controller.GetTodoItem(todoItem.TodoListId, todoItem.Id);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(todoItem, okResult.Value);
        }

        [TestMethod]
        public async Task PostTodoItem_ReturnsCreated_WhenSuccessful()
        {
            var listId = 2L;
            var payload = new CreateTodoItem { Description = "Buy milk" };
            var createdItem = new TodoItem { Id = 10, TodoListId = listId, Description = payload.Description };

            _todoItemServiceMock
                .Setup(s => s.CreateAsync(listId, payload.Description))
                .ReturnsAsync(createdItem);

            var result = await _controller.PostTodoItem(listId, payload);

            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdItem, createdResult.Value);
            Assert.AreEqual(nameof(TodoItemsController.GetTodoItem), createdResult.ActionName);
        }

        [TestMethod]
        public async Task PutTodoItem_ReturnsOk_WhenUpdated()
        {
            var listId = 3L;
            var itemId = 4L;
            var payload = new UpdateTodoItem { Description = "Updated" };
            var updatedItem = new TodoItem { Id = itemId, TodoListId = listId, Description = payload.Description };

            _todoItemServiceMock
                .Setup(s => s.UpdateDescriptionAsync(listId, itemId, payload.Description))
                .ReturnsAsync(updatedItem);

            var result = await _controller.PutTodoItem(listId, itemId, payload);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(updatedItem, okResult.Value);
        }

        [TestMethod]
        public async Task CompleteTodoItem_ReturnsOk_WhenCompleted()
        {
            var listId = 3L;
            var itemId = 4L;
            var completedItem = new TodoItem { Id = itemId, TodoListId = listId, Description = "Task", IsCompleted = true };

            _todoItemServiceMock
                .Setup(s => s.CompleteAsync(listId, itemId))
                .ReturnsAsync(completedItem);

            var result = await _controller.CompleteTodoItem(listId, itemId);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(completedItem, okResult.Value);
        }

        [TestMethod]
        public async Task DeleteTodoItem_ReturnsNoContent_WhenDeleted()
        {
            var listId = 5L;
            var itemId = 6L;

            _todoItemServiceMock
                .Setup(s => s.DeleteAsync(listId, itemId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteTodoItem(listId, itemId);

            var noContent = result as NoContentResult;
            Assert.IsNotNull(noContent);
            Assert.AreEqual(204, noContent.StatusCode);
        }
    }
}
