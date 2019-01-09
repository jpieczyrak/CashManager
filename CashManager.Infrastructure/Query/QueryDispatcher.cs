using System;

using log4net;

namespace CashManager.Infrastructure.Query
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(QueryDispatcher)));
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
                _logger.Value.Error("Execute", e);
                throw;
            }

            return handler.Execute(query);
        }
    }
}