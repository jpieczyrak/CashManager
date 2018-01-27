namespace Logic.Infrastructure.Command
{
    public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
    {
        void Execute(TCommand command);
    }

    public interface ICommandHandler
    {

    }
}