using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TodoDataAccess;
using TodoDataAccess.Implementation;
using TodoDomain.Entities;

namespace TodoApi.Tests.DataAcess
{
    [TestClass]
    public class TodoItemsDataAccessTests
    {
        private DbContextOptions<TodoContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        [TestMethod]
        public async Task GetByIdAsync_Returns_Item_When_Exists()
        {
            var dbName = nameof(GetByIdAsync_Returns_Item_When_Exists);
            long listId;
            long itemId;

            using (var seed = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList { Name = "List 1" };
                seed.TodoList.Add(list);
                await seed.SaveChangesAsync();
                listId = list.Id;

                var item = new TodoItem { TodoListId = listId, Description = "Test", IsCompleted = false };
                seed.TodoItem.Add(item);
                await seed.SaveChangesAsync();
                itemId = item.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.GetByIdAsync(listId, itemId);

                Assert.IsNotNull(result);
                Assert.AreEqual(itemId, result!.Id);
                Assert.AreEqual("Test", result.Description);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_Null_When_Not_Found()
        {
            var dbName = nameof(GetByIdAsync_Returns_Null_When_Not_Found);

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.GetByIdAsync(999, 999);

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public async Task AddAsync_Adds_Item_When_List_Exists()
        {
            var dbName = nameof(AddAsync_Adds_Item_When_List_Exists);
            long listId;

            using (var seed = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList { Name = "List" };
                seed.TodoList.Add(list);
                await seed.SaveChangesAsync();
                listId = list.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.AddAsync(listId, "New Item");

                Assert.IsNotNull(result);
                Assert.AreEqual("New Item", result!.Description);

                var fromDb = await context.TodoItem.FirstOrDefaultAsync(t => t.Id == result.Id);
                Assert.IsNotNull(fromDb);
            }
        }

        [TestMethod]
        public async Task AddAsync_Returns_Null_When_List_Does_Not_Exist()
        {
            var dbName = nameof(AddAsync_Returns_Null_When_List_Does_Not_Exist);

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.AddAsync(999, "Should fail");

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public async Task UpdateDescriptionAsync_Updates_When_Item_Exists()
        {
            var dbName = nameof(UpdateDescriptionAsync_Updates_When_Item_Exists);
            long listId;
            long itemId;

            using (var seed = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList { Name = "List" };
                seed.TodoList.Add(list);
                await seed.SaveChangesAsync();
                listId = list.Id;

                var item = new TodoItem { TodoListId = listId, Description = "Old", IsCompleted = false };
                seed.TodoItem.Add(item);
                await seed.SaveChangesAsync();
                itemId = item.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.UpdateDescriptionAsync(listId, itemId, "Updated");

                Assert.IsNotNull(result);
                Assert.AreEqual("Updated", result!.Description);

                var fromDb = await context.TodoItem.FirstOrDefaultAsync(t => t.Id == itemId);
                Assert.AreEqual("Updated", fromDb!.Description);
            }
        }

        [TestMethod]
        public async Task UpdateDescriptionAsync_Returns_Null_When_Not_Exist()
        {
            var dbName = nameof(UpdateDescriptionAsync_Returns_Null_When_Not_Exist);

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.UpdateDescriptionAsync(1, 1, "New");

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public async Task CompleteAsync_Sets_Completed_When_Exists()
        {
            var dbName = nameof(CompleteAsync_Sets_Completed_When_Exists);
            long listId;
            long itemId;

            using (var seed = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList { Name = "L" };
                seed.TodoList.Add(list);
                await seed.SaveChangesAsync();
                listId = list.Id;

                var item = new TodoItem { TodoListId = listId, Description = "Test", IsCompleted = false };
                seed.TodoItem.Add(item);
                await seed.SaveChangesAsync();
                itemId = item.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.CompleteAsync(listId, itemId);

                Assert.IsNotNull(result);
                Assert.IsTrue(result!.IsCompleted);

                var fromDb = await context.TodoItem.FirstOrDefaultAsync(t => t.Id == itemId);
                Assert.IsTrue(fromDb!.IsCompleted);
            }
        }

        [TestMethod]
        public async Task CompleteAsync_Returns_Null_When_Not_Exist()
        {
            var dbName = nameof(CompleteAsync_Returns_Null_When_Not_Exist);

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.CompleteAsync(1, 1);

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_Removes_Item_When_Exists()
        {
            var dbName = nameof(DeleteAsync_Removes_Item_When_Exists);
            long listId;
            long itemId;

            using (var seed = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList { Name = "L" };
                seed.TodoList.Add(list);
                await seed.SaveChangesAsync();
                listId = list.Id;

                var item = new TodoItem { TodoListId = listId, Description = "Delete", IsCompleted = false };
                seed.TodoItem.Add(item);
                await seed.SaveChangesAsync();
                itemId = item.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var ok = await repo.DeleteAsync(listId, itemId);

                Assert.IsTrue(ok);

                var fromDb = await context.TodoItem.FirstOrDefaultAsync(t => t.Id == itemId);
                Assert.IsNull(fromDb);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_Returns_False_When_Not_Exist()
        {
            var dbName = nameof(DeleteAsync_Returns_False_When_Not_Exist);

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoItemRepository(context);

                var result = await repo.DeleteAsync(1, 1);

                Assert.IsFalse(result);
            }
        }
    }
}
