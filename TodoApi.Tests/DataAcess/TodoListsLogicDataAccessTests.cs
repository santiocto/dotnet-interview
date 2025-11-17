using Microsoft.EntityFrameworkCore;
using TodoDataAccess;
using TodoDataAccess.Implementation;
using TodoDomain.Entities;

namespace TodoApi.Tests.DataAcess
{
    [TestClass]
    public class TodoListsLogicDataAccessTests
    {
        private DbContextOptions<TodoContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        [TestMethod]
        public async Task GetAsync_Returns_All_Lists_With_Items()
        {
            var dbName = nameof(GetAsync_Returns_All_Lists_With_Items);
            using (var seedContext = new TodoContext(CreateOptions(dbName)))
            {
                var list1 = new TodoList
                {
                    Name = "List 1",
                    Items = new List<TodoItem>
                    {
                        new TodoItem { Description = "Item 1.1", IsCompleted = false },
                        new TodoItem { Description = "Item 1.2", IsCompleted = true }
                    }
                };

                var list2 = new TodoList
                {
                    Name = "List 2",
                    Items = new List<TodoItem>
                    {
                        new TodoItem { Description = "Item 2.1", IsCompleted = false }
                    }
                };

                seedContext.TodoList.AddRange(list1, list2);
                await seedContext.SaveChangesAsync();
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var result = await repo.GetAsync();

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("List 1", result[0].Name);
                Assert.AreEqual(2, result[0].Items.Count);
                Assert.AreEqual("List 2", result[1].Name);
                Assert.AreEqual(1, result[1].Items.Count);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_List_With_Items_When_Exists()
        {
            var dbName = nameof(GetByIdAsync_Returns_List_With_Items_When_Exists);
            long listId;

            using (var seedContext = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList
                {
                    Name = "List 1",
                    Items = new List<TodoItem>
                    {
                        new TodoItem { Description = "Item 1.1", IsCompleted = false }
                    }
                };

                seedContext.TodoList.Add(list);
                await seedContext.SaveChangesAsync();
                listId = list.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var result = await repo.GetByIdAsync(listId);

                Assert.IsNotNull(result);
                Assert.AreEqual(listId, result!.Id);
                Assert.AreEqual("List 1", result.Name);
                Assert.AreEqual(1, result.Items.Count);
                Assert.AreEqual("Item 1.1", result.Items.First().Description);
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_Null_When_Not_Found()
        {
            var dbName = nameof(GetByIdAsync_Returns_Null_When_Not_Found);
            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var result = await repo.GetByIdAsync(999);

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public async Task AddAsync_Persists_List()
        {
            var dbName = nameof(AddAsync_Persists_List);
            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var list = new TodoList { Name = "New List" };

                var created = await repo.AddAsync(list);

                Assert.IsTrue(created.Id > 0);
                Assert.AreEqual("New List", created.Name);

                var fromDb = await context.TodoList.FirstOrDefaultAsync(t => t.Id == created.Id);
                Assert.IsNotNull(fromDb);
                Assert.AreEqual("New List", fromDb!.Name);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_Updates_Name_When_List_Exists()
        {
            var dbName = nameof(UpdateAsync_Updates_Name_When_List_Exists);
            long listId;

            using (var seedContext = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList { Name = "Old Name" };
                seedContext.TodoList.Add(list);
                await seedContext.SaveChangesAsync();
                listId = list.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var updated = await repo.UpdateAsync(listId, "New Name");

                Assert.IsNotNull(updated);
                Assert.AreEqual(listId, updated!.Id);
                Assert.AreEqual("New Name", updated.Name);

                var fromDb = await context.TodoList.FirstOrDefaultAsync(t => t.Id == listId);
                Assert.IsNotNull(fromDb);
                Assert.AreEqual("New Name", fromDb!.Name);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_Returns_Null_When_List_Does_Not_Exist()
        {
            var dbName = nameof(UpdateAsync_Returns_Null_When_List_Does_Not_Exist);
            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var updated = await repo.UpdateAsync(999, "New Name");

                Assert.IsNull(updated);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_Removes_List_When_Exists()
        {
            var dbName = nameof(DeleteAsync_Removes_List_When_Exists);
            long listId;

            using (var seedContext = new TodoContext(CreateOptions(dbName)))
            {
                var list = new TodoList { Name = "List to delete" };
                seedContext.TodoList.Add(list);
                await seedContext.SaveChangesAsync();
                listId = list.Id;
            }

            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var result = await repo.DeleteAsync(listId);

                Assert.IsTrue(result);

                var fromDb = await context.TodoList.FirstOrDefaultAsync(t => t.Id == listId);
                Assert.IsNull(fromDb);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_Returns_False_When_List_Does_Not_Exist()
        {
            var dbName = nameof(DeleteAsync_Returns_False_When_List_Does_Not_Exist);
            using (var context = new TodoContext(CreateOptions(dbName)))
            {
                var repo = new TodoListRepository(context);

                var result = await repo.DeleteAsync(999);

                Assert.IsFalse(result);
            }
        }
    }
}
