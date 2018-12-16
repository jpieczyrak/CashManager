using System;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.Categories
{
    public class CategoryTests
    {
        [Fact]
        public void CategoryQueryHandler_CategoryQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new CategoryQueryHandler(repository);
            var query = new CategoryQuery();

            //when
            var result = handler.Execute(query);

            //then
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CategoryQueryHandler_CategoryQueryNotEmptyDatabase_Array()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new CategoryQueryHandler(repository);
            var query = new CategoryQuery();
            var rootCategory = new Category(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1))
            {
                Name = "1",
                Parent = null
            };
            var categories = new[]
            {
                rootCategory,
                new Category(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2))
                {
                    Name = "2",
                    Parent = rootCategory
                }
            };
            repository.Database.GetCollection<Category>().InsertBulk(categories);

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(categories, result);
        }
    }
}