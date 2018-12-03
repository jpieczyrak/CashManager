using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Categories
{
    public class DeleteCommandTests
    {
        [Fact]
        public void DeleteCategoryCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteCategoryCommandHandler(repository);
            var command = new DeleteCategoryCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Category>());
        }

        [Fact]
        public void DeleteCategoryCommandHandler_NotExisting_NoChange()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteCategoryCommandHandler(repository);
            var command = new DeleteCategoryCommand(new Category());

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Category>());
        }

        [Fact]
        public void DeleteCategoryCommandHandler_Existing_Removed()
        {
            //given
            var category = new Category();
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteCategoryCommandHandler(repository);
            var command = new DeleteCategoryCommand(category);
            repository.Database.Upsert(category);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Category>());
        }

        [Fact]
        public void DeleteCategoryCommandHandler_MoreObjects_RemovedProperOne()
        {
            //given
            var targetCategory = new Category();
            var categories = new[] { targetCategory, new Category(), new Category(), new Category() };
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteCategoryCommandHandler(repository);
            var command = new DeleteCategoryCommand(targetCategory);
            repository.Database.UpsertBulk(categories);

            //when
            handler.Execute(command);

            //then
            categories = categories.Skip(1).OrderBy(x => x.Id).ToArray();
            var actualCategories = repository.Database.Query<Category>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(categories.Length, actualCategories.Length);
            Assert.Equal(categories, actualCategories);
        }
    }
}