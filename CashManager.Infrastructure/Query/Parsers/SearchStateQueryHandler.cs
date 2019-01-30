using CashManager.Data.ViewModelState.Parsers;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.Parsers
{
	public class CustomCsvParserQueryHandler : IQueryHandler<CustomCsvParserQuery, CustomCsvParser[]>
	{
		private readonly LiteDatabase _db;

		public CustomCsvParserQueryHandler(LiteRepository repository) => _db = repository.Database;

		public CustomCsvParser[] Execute(CustomCsvParserQuery query) => _db.Query(query.Query);
	}
}
