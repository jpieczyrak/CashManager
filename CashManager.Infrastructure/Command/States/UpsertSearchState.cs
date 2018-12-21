using CashManager.Data.ViewModelState;

namespace CashManager.Infrastructure.Command.States
{
    public class UpsertSearchState : ICommand
    {
        public SearchState SearchState { get; }

        public UpsertSearchState(SearchState searchState)
        {
            SearchState = searchState;
        }
    }
}