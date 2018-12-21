using System;
using System.Linq.Expressions;

using CashManager.Data.ViewModelState;

namespace CashManager.Infrastructure.Query.States
{
    public class SearchStateQuery : IQuery<SearchState[]>
    {
        public Expression<Func<SearchState, bool>> Query { get; }

        public SearchStateQuery(Expression<Func<SearchState, bool>> query = null)
        {
            Query = query;
        }
    }
}