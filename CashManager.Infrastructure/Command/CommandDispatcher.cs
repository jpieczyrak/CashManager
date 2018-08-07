using System;

namespace CashManager.Infrastructure.Command
{
    public class CommandDispatcher : ICommandDispatcher
    {
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
                throw e;
            }

            handler.Execute(command);
        }
    }
}