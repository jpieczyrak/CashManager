namespace Logic.Infrastructure.Command
{
    public class NoCommandHandler : ICommandHandler<NoCommand>
    {
        public void Execute(NoCommand command)
        {
        }
    }
}