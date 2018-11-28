using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Query.Tags
{
	public class TagQueryHandler : IQueryHandler<TagQuery, Tag[]>
	{
		private readonly LiteDatabase _db;

		public TagQueryHandler(LiteRepository repository) => _db = repository.Database;

		public Tag[] Execute(TagQuery query) => _db.Read<Tag>();
	}
}
