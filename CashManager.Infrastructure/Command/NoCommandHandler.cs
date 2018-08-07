namespace CashManager.Infrastructure.Command
{
    public class NoCommandHandler : ICommandHandler<NoCommand>
    {
        public void Execute(NoCommand command)
        {
        }
    }
}