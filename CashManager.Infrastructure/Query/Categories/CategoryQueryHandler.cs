using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.Categories
{
	public class CategoryQueryHandler : IQueryHandler<CategoryQuery, Category[]>
	{
		private readonly LiteDatabase _db;

		public CategoryQueryHandler(LiteRepository repository) => _db = repository.Database;

		public Category[] Execute(CategoryQuery query) => _db.Read<Category>();
	}
}
