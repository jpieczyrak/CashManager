using System.Linq;

using CashManager.Data.DTO;

using LiteDB;

namespace CashManager.Infrastructure.Query.Categories
{
	public class CategoryQueryHandler : IQueryHandler<CategoryQuery, Category[]>
	{
		private readonly LiteRepository _repository;

		public CategoryQueryHandler(LiteRepository repository) => _repository = repository;

		public Category[] Execute(CategoryQuery query) => _repository.Database.GetCollection<Category>().FindAll().ToArray();
	}
}
