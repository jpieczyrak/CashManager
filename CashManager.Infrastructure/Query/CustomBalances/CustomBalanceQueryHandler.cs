using CashManager.Data.ViewModelState.Balances;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.CustomBalances
{
	public class CustomBalanceQueryHandler : IQueryHandler<CustomBalanceQuery, CustomBalance[]>
	{
		private readonly LiteDatabase _db;

		public CustomBalanceQueryHandler(LiteRepository repository) => _db = repository.Database;

		public CustomBalance[] Execute(CustomBalanceQuery query) => _db.Query<CustomBalance>();
	}
}
