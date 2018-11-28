using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Categories
{
    public class UpsertCommandTests
    {
        [Fact]
        public void UpsertCategoriesCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertCategoriesCommandHandler(repository);
            var command = new UpsertCategoriesCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Category>());
        }

        [Fact]
        public void UpsertCategoriesCommandHandler_EmptyDbUpsertOneObject_ObjectSaved()
        {
            //given
            var categories = new[]
            {
                new Category { Value = "test1" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertCategoriesCommandHandler(repository);
            var command = new UpsertCategoriesCommand(categories);

            //when
            handler.Execute(command);

            //then
            var orderedCategoryInDatabase = repository.Database.Query<Category>().OrderBy(x => x.Id);
            Assert.Equal(categories.OrderBy(x => x.Id), orderedCategoryInDatabase);
        }

        [Fact]
        public void UpsertCategoriesCommandHandler_EmptyDbUpsertList_ListSaved()
        {
            //given
            var categories = new[]
            {
                new Category { Value = "test1" },
                new Category { Value = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertCategoriesCommandHandler(repository);
            var command = new UpsertCategoriesCommand(categories);

            //when
            handler.Execute(command);

            //then
            var orderedCategoryInDatabase = repository.Database.Query<Category>().OrderBy(x => x.Id);
            Assert.Equal(categories.OrderBy(x => x.Id), orderedCategoryInDatabase);
        }

        [Fact]
        public void UpsertCategoriesCommandHandler_NotEmptyDbUpsertList_ListUpdated()
        {
            //given
            var categories = new[]
            {
                new Category { Value = "test1" },
                new Category { Value = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertCategoriesCommandHandler(repository);
            var command = new UpsertCategoriesCommand(categories);
            repository.Database.UpsertBulk(categories);
            foreach (var category in categories) category.Value += " - updated";

            //when
            handler.Execute(command);

            //then
            var orderedCategoryInDatabase = repository.Database.Query<Category>().OrderBy(x => x.Id).ToArray();
            categories = categories.OrderBy(x => x.Id).ToArray();
            Assert.Equal(categories, orderedCategoryInDatabase);
            for (int i = 0; i < categories.Length; i++) Assert.Equal(categories[i].Value, orderedCategoryInDatabase[i].Value);
        }
    }
}