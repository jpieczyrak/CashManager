namespace CashManager.Infrastructure.Command.NoCommands
{
    public class NoCommandHandler : ICommandHandler<NoCommand>
    {
        public void Execute(NoCommand command)
        {
        }
    }
}