using System.Collections.Generic;

namespace CashManager.Infrastructure.Query
{
    public class NoQueryHandler : IQueryHandler<NoQuery, IEnumerable<string>>
    {
        public IEnumerable<string> Execute(NoQuery query)
        {
            return new List<string>();
        }
    }
}