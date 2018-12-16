using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.TransactionTypes
{
	public class TransactionTypesQueryHandler : IQueryHandler<TransactionTypesQuery, TransactionType[]>
	{
		private readonly LiteDatabase _db;

		public TransactionTypesQueryHandler(LiteRepository repository) => _db = repository.Database;

		public TransactionType[] Execute(TransactionTypesQuery query) => _db.Query(query.Query);
	}
}
