using CashManager.Data.ViewModelState;

namespace CashManager.Infrastructure.Command.States
{
    public class UpsertSearchStateCommand : ICommand
    {
        public SearchState[] SearchStates { get; }

        public UpsertSearchStateCommand(SearchState searchState)
        {
            SearchStates = new []{ searchState };
        }

        public UpsertSearchStateCommand(SearchState[] searchStates)
        {
            SearchStates = searchStates;
        }
    }
}