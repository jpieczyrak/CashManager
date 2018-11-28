using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.Transactions
{
	public class TransactionQueryHandler : IQueryHandler<TransactionQuery, Transaction[]>
	{
		private readonly LiteDatabase _db;

		public TransactionQueryHandler(LiteRepository repository) => _db = repository.Database;

		public Transaction[] Execute(TransactionQuery query) => _db.Query<Transaction>();
	}
}
