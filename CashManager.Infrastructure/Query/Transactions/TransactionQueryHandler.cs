using System.Linq;

using CashManager.Data.DTO;

using LiteDB;

namespace CashManager.Infrastructure.Query.Transactions
{
	public class TransactionQueryHandler : IQueryHandler<TransactionQuery, Transaction[]>
	{
		private readonly LiteRepository _repository;

		public TransactionQueryHandler(LiteRepository repository) => _repository = repository;

		public Transaction[] Execute(TransactionQuery query) => _repository.Database.GetCollection<Transaction>().FindAll().ToArray();
	}
}
