using System.Collections.Generic;

namespace CashManager.Infrastructure.Query.NoQueries
{
	public class NoQueryHandler : IQueryHandler<NoQuery, IEnumerable<string>>
	{
		public IEnumerable<string> Execute(NoQuery query) => new List<string>();
	}
}
