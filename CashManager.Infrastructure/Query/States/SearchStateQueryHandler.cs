using CashManager.Data.ViewModelState;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.States
{
	public class SearchStateQueryHandler : IQueryHandler<SearchStateQuery, SearchState[]>
	{
		private readonly LiteDatabase _db;

		public SearchStateQueryHandler(LiteRepository repository) => _db = repository.Database;

		public SearchState[] Execute(SearchStateQuery query) => _db.Query<SearchState>();
	}
}
