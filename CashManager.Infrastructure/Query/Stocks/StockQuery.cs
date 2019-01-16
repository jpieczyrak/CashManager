using System;
using System.Linq.Expressions;

using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Query.Stocks
{
    public class StockQuery : IQuery<Stock[]>
    {
        public Expression<Func<Stock, bool>> Query { get; }

        public StockQuery(Expression<Func<Stock, bool>> query = null) { Query = query; }
    }
}