using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoDomain.Dtos;
using TodoDomain.Entities;
using TodoLogic.Interfaces;

namespace TodoApiTests.Controllers
{
    [TestClass]
    public class TodoListsControllerTests
    {
        private Mock<ITodoListService> _todoListServiceMock;
        private TodoListsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _todoListServiceMock = new Mock<ITodoListService>(MockBehavior.Strict);
            _controller = new TodoListsController(_todoListServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _todoListServiceMock.VerifyAll();
        }

        [TestMethod]
        public async Task GetTodoLists_ReturnsOk_WithList()
        {
            // Arrange
            var lists = new List<TodoList>
            {
                new TodoList { Id = 1, Name = "List 1" },
                new TodoList { Id = 2, Name = "List 2" }
            };

            _todoListServiceMock
                .Setup(s => s.GetAsync())
                .ReturnsAsync(lists);

            var result = await _controller.GetTodoLists();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreSame(lists, okResult.Value);
        }

        // GET: api/todolists/5
        [TestMethod]
        public async Task GetTodoList_ReturnsOk_WhenExists()
        {
            // Arrange
            var id = 1L;
            var list = new TodoList { Id = id, Name = "My list" };

            _todoListServiceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(list);

            // Act
            var result = await _controller.GetTodoList(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreSame(list, okResult.Value);
        }

        // PUT: api/todolists/5
        [TestMethod]
        public async Task PutTodoList_ReturnsOk_WhenUpdated()
        {
            // Arrange
            var id = 1L;
            var payload = new UpdateTodoList { Name = "Updated name" };
            var updatedList = new TodoList { Id = id, Name = payload.Name };

            _todoListServiceMock
                .Setup(s => s.UpdateAsync(id, payload.Name))
                .ReturnsAsync(updatedList);

            // Act
            var result = await _controller.PutTodoList(id, payload);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreSame(updatedList, okResult.Value);
        }

        // POST: api/todolists
        [TestMethod]
        public async Task PostTodoList_ReturnsCreatedAt_WithNewList()
        {
            // Arrange
            var payload = new CreateTodoList { Name = "New list" };
            var createdList = new TodoList { Id = 10, Name = payload.Name };

            _todoListServiceMock
                .Setup(s => s.CreateAsync(payload.Name))
                .ReturnsAsync(createdList);

            // Act
            var result = await _controller.PostTodoList(payload);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(nameof(TodoListsController.GetTodoList), createdResult.ActionName);
            Assert.AreSame(createdList, createdResult.Value);
        }

        // DELETE: api/todolists/5
        [TestMethod]
        public async Task DeleteTodoList_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var id = 1L;

            _todoListServiceMock
                .Setup(s => s.DeleteAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTodoList(id);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }
    }
}
