using System;
using System.IO;

using CashManager.Infrastructure.Query.Categories;

using LiteDB;

using Xunit;

namespace CashManagerTests.Infrastructure.Queries.Categories
{
	public class CategoryTests
	{
		[Fact]
		public void CategoryQueryHandler_CategoryQueryEmptyDatabase_EmptyArray()
		{
            //given
			var repository = GetEmptyDatabase();
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
			var repository = GetEmptyDatabase();
            var handler = new CategoryQueryHandler(repository);
			var query = new CategoryQuery();
			var rootCategory = new CashManager.Data.DTO.Category
			{
				Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1),
				Value = "1",
				Parent = null
			};
			var categories = new[]
			{
				rootCategory,
				new CashManager.Data.DTO.Category
				{
					Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2),
					Value = "2",
					Parent = rootCategory
                }
			};
			repository.Database.GetCollection<CashManager.Data.DTO.Category>().InsertBulk(categories);

			//when
			var result = handler.Execute(query);

			//then
			Assert.Equal(categories, result);
		}

		private static LiteRepository GetEmptyDatabase()
		{
			return new LiteRepository(new LiteDatabase(new MemoryStream()));
		}
    }
}
