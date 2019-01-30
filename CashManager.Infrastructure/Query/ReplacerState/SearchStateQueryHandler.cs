using CashManager.Data.ViewModelState;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.ReplacerState
{
	public class ReplacerStateQueryHandler : IQueryHandler<ReplacerStateQuery, MassReplacerState[]>
	{
		private readonly LiteDatabase _db;

		public ReplacerStateQueryHandler(LiteRepository repository) => _db = repository.Database;

		public MassReplacerState[] Execute(ReplacerStateQuery query) => _db.Query(query.Query);
	}
}
