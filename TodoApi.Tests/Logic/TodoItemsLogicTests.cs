using Microsoft.Extensions.DependencyInjection;
using TodoDataAccess.Interfaces;
using TodoDomain.Entities;
using TodoLogic.Implementation;
using TodoLogic.Interfaces;

namespace TodoApi.Tests.Logic
{
    [TestClass]
    public class TodoItemsLogicTests
    {
        private ServiceProvider _serviceProvider = null!;
        private Mock<ITodoItemRepository> _todoItemRepositoryMock = null!;
        private ITodoItemService _service = null!;

        [TestInitialize]
        public void SetUp()
        {
            _todoItemRepositoryMock = new Mock<ITodoItemRepository>(MockBehavior.Strict);

            var services = new ServiceCollection();
            services.AddSingleton(_ => _todoItemRepositoryMock.Object);
            services.AddTransient<ITodoItemService, TodoItemService>();

            _serviceProvider = services.BuildServiceProvider();
            _service = _serviceProvider.GetRequiredService<ITodoItemService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _todoItemRepositoryMock.VerifyAll();
            _serviceProvider.Dispose();
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_Item_When_Exists()
        {
            const long listId = 1;
            const long itemId = 10;

            var item = new TodoItem
            {
                Id = itemId,
                TodoListId = listId,
                Description = "Test item",
                IsCompleted = false
            };

            _todoItemRepositoryMock
                .Setup(r => r.GetByIdAsync(listId, itemId))
                .ReturnsAsync(item);

            var result = await _service.GetByIdAsync(listId, itemId);

            Assert.IsNotNull(result);
            Assert.AreEqual(itemId, result!.Id);
            Assert.AreEqual(listId, result.TodoListId);
            Assert.AreEqual("Test item", result.Description);
            Assert.IsFalse(result.IsCompleted);
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_Null_When_NotFound()
        {
            const long listId = 1;
            const long itemId = 999;

            _todoItemRepositoryMock
                .Setup(r => r.GetByIdAsync(listId, itemId))
                .ReturnsAsync((TodoItem?)null);

            var result = await _service.GetByIdAsync(listId, itemId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateAsync_Creates_Item_In_List()
        {
            const long listId = 1;
            const string description = "New item";

            _todoItemRepositoryMock
                .Setup(r => r.AddAsync(listId, description))
                .ReturnsAsync(new TodoItem
                {
                    Id = 42,
                    TodoListId = listId,
                    Description = description,
                    IsCompleted = false
                });

            var result = await _service.CreateAsync(listId, description);

            Assert.IsNotNull(result);
            Assert.AreEqual(42, result!.Id);
            Assert.AreEqual(listId, result.TodoListId);
            Assert.AreEqual(description, result.Description);
            Assert.IsFalse(result.IsCompleted);

            _todoItemRepositoryMock.Verify(r =>
                r.AddAsync(listId, description), Times.Once);
        }

        [TestMethod]
        public async Task UpdateDescriptionAsync_Updates_Description_When_Item_Exists()
        {
            const long listId = 1;
            const long itemId = 10;
            const string newDescription = "Updated description";

            var updatedItem = new TodoItem
            {
                Id = itemId,
                TodoListId = listId,
                Description = newDescription,
                IsCompleted = false
            };

            _todoItemRepositoryMock
                .Setup(r => r.UpdateDescriptionAsync(listId, itemId, newDescription))
                .ReturnsAsync(updatedItem);

            var result = await _service.UpdateDescriptionAsync(listId, itemId, newDescription);

            Assert.IsNotNull(result);
            Assert.AreEqual(itemId, result!.Id);
            Assert.AreEqual(listId, result.TodoListId);
            Assert.AreEqual(newDescription, result.Description);
        }

        [TestMethod]
        public async Task CompleteAsync_Marks_Item_As_Completed()
        {
            const long listId = 1;
            const long itemId = 10;

            var completedItem = new TodoItem
            {
                Id = itemId,
                TodoListId = listId,
                Description = "Item",
                IsCompleted = true
            };

            _todoItemRepositoryMock
                .Setup(r => r.CompleteAsync(listId, itemId))
                .ReturnsAsync(completedItem);

            var result = await _service.CompleteAsync(listId, itemId);

            Assert.IsNotNull(result);
            Assert.AreEqual(itemId, result!.Id);
            Assert.AreEqual(listId, result.TodoListId);
            Assert.IsTrue(result.IsCompleted);
        }

        [TestMethod]
        public async Task DeleteAsync_Delegates_To_Repository_And_Returns_Result()
        {
            const long listId = 1;
            const long itemId = 10;

            _todoItemRepositoryMock
                .Setup(r => r.DeleteAsync(listId, itemId))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(listId, itemId);

            Assert.IsTrue(result);
        }
    }
}
