using System;
using System.Linq.Expressions;

using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Query.Transactions
{
    public class TransactionQuery : IQuery<Transaction[]>
    {
        public Expression<Func<Transaction, bool>> Query { get; }

        public TransactionQuery(Expression<Func<Transaction, bool>> query = null)
        {
            Query = query;
        }
    }
}
