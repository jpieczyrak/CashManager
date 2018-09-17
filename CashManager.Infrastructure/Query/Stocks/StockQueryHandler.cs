using System.Linq;

using CashManager.Data.DTO;

using LiteDB;

namespace CashManager.Infrastructure.Query.Stocks
{
	public class StockQueryHandler : IQueryHandler<StockQuery, Stock[]>
	{
		private readonly LiteRepository _repository;

		public StockQueryHandler(LiteRepository repository) => _repository = repository;

		public Stock[] Execute(StockQuery query) => _repository.Database.GetCollection<Stock>().FindAll().ToArray();
	}
}
