using System;

using log4net;

namespace CashManager.Infrastructure.Command
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(CommandDispatcher)));
        private readonly Func<Type, ICommandHandler> _handlersFactory;

        public CommandDispatcher(Func<Type, ICommandHandler> handlersFactory)
        {
            _handlersFactory = handlersFactory;
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            ICommandHandler<TCommand> handler;

            try
            {
                handler = (ICommandHandler<TCommand>)_handlersFactory(typeof(TCommand));
            }
            catch (Exception e)
            {
                _logger.Value.Error("Execute", e);
                throw e;
            }

            handler.Execute(command);
        }
    }
}