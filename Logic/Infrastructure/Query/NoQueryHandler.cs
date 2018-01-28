using System.Collections.Generic;

namespace Logic.Infrastructure.Query
{
    public class NoQueryHandler : IQueryHandler<NoQuery, IEnumerable<string>>
    {
        public IEnumerable<string> Execute(NoQuery query)
        {
            return new List<string>();
        }
    }
}