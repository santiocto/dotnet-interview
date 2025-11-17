using Microsoft.Extensions.DependencyInjection;
using TodoDataAccess.Interfaces;
using TodoDomain.Entities;
using TodoLogic.Implementation;
using TodoLogic.Interfaces;

namespace TodoApi.Tests.Logic
{
    [TestClass]
    public class TodoListsLogicTests
    {
        private ServiceProvider _serviceProvider = null!;
        private Mock<ITodoListRepository> _todoListRepositoryMock = null!;
        private ITodoListService _service = null!;

        [TestInitialize]
        public void SetUp()
        {
            _todoListRepositoryMock = new Mock<ITodoListRepository>(MockBehavior.Strict);

            var services = new ServiceCollection();

            services.AddSingleton(_ => _todoListRepositoryMock.Object);

            services.AddTransient<ITodoListService, TodoListService>();

            _serviceProvider = services.BuildServiceProvider();

            _service = _serviceProvider.GetRequiredService<ITodoListService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _todoListRepositoryMock.VerifyAll();
            _serviceProvider.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_Returns_Lists_From_Repository()
        {
            var lists = new List<TodoList>
            {
                new TodoList { Id = 1, Name = "List 1" },
                new TodoList { Id = 2, Name = "List 2" }
            }.AsReadOnly();

            _todoListRepositoryMock
                .Setup(r => r.GetAsync())
                .ReturnsAsync(lists);

            var result = await _service.GetAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("List 1", result[0].Name);
            Assert.AreEqual("List 2", result[1].Name);
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_List_When_Exists()
        {
            var list = new TodoList { Id = 1, Name = "List 1" };

            _todoListRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(list);

            var result = await _service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.Id);
            Assert.AreEqual("List 1", result.Name);
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_Null_When_NotFound()
        {
            _todoListRepositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((TodoList?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateAsync_Creates_List_And_Passes_To_Repository()
        {
            const string name = "New List";

            _todoListRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<TodoList>()))
                .ReturnsAsync((TodoList l) =>
                {
                    l.Id = 42;
                    return l;
                });

            var result = await _service.CreateAsync(name);

            Assert.AreEqual(42, result.Id);
            Assert.AreEqual(name, result.Name);

            _todoListRepositoryMock.Verify(r =>
                    r.AddAsync(It.Is<TodoList>(l => l.Name == name)),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_Delegates_To_Repository()
        {
            const long id = 1;
            const string newName = "Updated Name";

            var updated = new TodoList { Id = id, Name = newName };

            _todoListRepositoryMock
                .Setup(r => r.UpdateAsync(id, newName))
                .ReturnsAsync(updated);

            var result = await _service.UpdateAsync(id, newName);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result!.Id);
            Assert.AreEqual(newName, result.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_Delegates_To_Repository_And_Returns_Result()
        {
            const long id = 1;

            _todoListRepositoryMock
                .Setup(r => r.DeleteAsync(id))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(id);

            Assert.IsTrue(result);
        }
    }
}
