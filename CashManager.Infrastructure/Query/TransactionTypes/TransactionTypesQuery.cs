using System;
using System.Linq.Expressions;

using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Query.TransactionTypes
{
    public class TransactionTypesQuery : IQuery<TransactionType[]>
    {
        public Expression<Func<TransactionType, bool>> Query { get; }

        public TransactionTypesQuery(Expression<Func<TransactionType, bool>> query = null)
        {
            Query = query;
        }
    }
}
