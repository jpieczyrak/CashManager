using System;

namespace Logic.Infrastructure.Query
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly Func<Type, IQueryHandler> _handlersFactory;

        public QueryDispatcher(Func<Type, IQueryHandler> handlersFactory)
        {
            _handlersFactory = handlersFactory;
        }

        public TResult Execute<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            IQueryHandler<TQuery, TResult> handler;

            try
            {
                handler = (IQueryHandler<TQuery, TResult>) _handlersFactory(typeof(TQuery));
            }
            catch (Exception e)
            {
                throw e;
            }

            return handler.Execute(query);
        }
    }
}