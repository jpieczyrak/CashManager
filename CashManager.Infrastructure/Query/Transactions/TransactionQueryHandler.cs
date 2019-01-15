using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.Transactions
{
	public class TransactionQueryHandler : IQueryHandler<TransactionQuery, Transaction[]>
	{
	    private const string POSITIONS_NAME = nameof(Transaction.Positions);
        private const string CATEGORY_NAME = nameof(Position.Category);
	    private const string PARENT_NAME = nameof(Category.Parent);
	    private const string TAGS_NAME = nameof(Position.Tags);
        private readonly LiteDatabase _db;

		public TransactionQueryHandler(LiteRepository repository) => _db = repository.Database;

	    public Transaction[] Execute(TransactionQuery query)
	    {
	        var collection = _db.GetCollection<Transaction>()
	                            .Include(x => x.ExternalStock)
	                            .Include(x => x.UserStock)
	                            .Include(x => x.Type)
	                            .Include($"$.{POSITIONS_NAME}[*].{TAGS_NAME}[*]")
	                            .Include($"$.{POSITIONS_NAME}[*].{CATEGORY_NAME}")
	                            .Include($"$.{POSITIONS_NAME}[*].{CATEGORY_NAME}.{PARENT_NAME}");

	        return query.Query != null
	                   ? collection.Find(query.Query).ToArray()
	                   : collection.FindAll().ToArray();
	    }
	}
}
