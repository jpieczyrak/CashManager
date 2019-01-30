using System.Linq;

using CashManager.Data.DTO;

using LiteDB;

namespace CashManager.Infrastructure.Query.Transactions
{
	public class TransactionQueryHandler : IQueryHandler<TransactionQuery, Transaction[]>
	{
        private readonly LiteDatabase _db;

		public TransactionQueryHandler(LiteRepository repository) => _db = repository.Database;

	    public Transaction[] Execute(TransactionQuery query)
	    {
	        var collection = _db.GetCollection<Transaction>();

	        return query.Query != null
	                   ? collection.Find(query.Query).ToArray()
	                   : collection.FindAll().ToArray();
	    }
	}
}
