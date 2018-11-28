using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.Stocks
{
	public class StockQueryHandler : IQueryHandler<StockQuery, Stock[]>
	{
		private readonly LiteDatabase _db;

		public StockQueryHandler(LiteRepository repository) => _db = repository.Database;

		public Stock[] Execute(StockQuery query) => _db.Read<Stock>();
	}
}
