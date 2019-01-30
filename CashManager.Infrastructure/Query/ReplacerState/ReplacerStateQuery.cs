using System;
using System.Linq.Expressions;

using CashManager.Data.ViewModelState;

namespace CashManager.Infrastructure.Query.ReplacerState
{
    public class ReplacerStateQuery : IQuery<MassReplacerState[]>
    {
        public Expression<Func<MassReplacerState, bool>> Query { get; }

        public ReplacerStateQuery(Expression<Func<MassReplacerState, bool>> query = null)
        {
            Query = query;
        }
    }
}