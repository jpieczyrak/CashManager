using System.IO;

using LiteDB;

namespace CashManager.Infrastructure.Query.Transactions.Bills
{
	public class BillQueryHandler : IQueryHandler<BillQuery, byte[]>
	{
		private readonly LiteDatabase _db;

		public BillQueryHandler(LiteRepository repository) => _db = repository.Database;

		public byte[] Execute(BillQuery query)
	    {
	        var file = _db.FileStorage.FindById(query.Id);
	        if (file == null) return null;

	        using (var stream = new MemoryStream())
	        {
	            file.CopyTo(stream);
	            return stream.ToArray();
	        }
	    }
	}
}
