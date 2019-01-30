using CashManager.Data.ViewModelState;

namespace CashManager.Infrastructure.Command.ReplacerState
{
    public class UpsertReplacerStateCommand : ICommand
    {
        public MassReplacerState State { get; }

        public UpsertReplacerStateCommand(MassReplacerState state)
        {
            State = state;
        }
    }
}