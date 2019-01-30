using System;
using System.Linq.Expressions;

using CashManager.Data.ViewModelState.Parsers;

namespace CashManager.Infrastructure.Query.Parsers
{
    public class CustomCsvParserQuery : IQuery<CustomCsvParser[]>
    {
        public Expression<Func<CustomCsvParser, bool>> Query { get; }

        public CustomCsvParserQuery(Expression<Func<CustomCsvParser, bool>> query = null)
        {
            Query = query;
        }
    }
}