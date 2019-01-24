using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;

using CashManager_MVVM.Features.Search;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Transaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.MassReplacer
{
    public class MassReplacerViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public SearchViewModel SearchViewModel { get; private set; }

        public ReplacerState State { get; private set; }

        public RelayCommand PerformCommand { get; }

        public MassReplacerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            State = new ReplacerState();
            SearchViewModel = factory.Create<SearchViewModel>();
            PerformCommand = new RelayCommand(ExecutePerformCommand, CanExecutePerformCommand);
        }

        private bool CanExecutePerformCommand()
        {
            return (State.BookDateSetter.IsChecked
                     || (State.UserStocksSelector.IsChecked && State.UserStocksSelector.Selected != null)
                     || (State.ExternalStocksSelector.IsChecked && State.ExternalStocksSelector.Selected != null)
                     || (State.TitleSelector.IsChecked && !string.IsNullOrWhiteSpace(State.TitleSelector.Value))
                     || State.NoteSelector.IsChecked
                     || (State.PositionTitleSelector.IsChecked && !string.IsNullOrWhiteSpace(State.PositionTitleSelector.Value))
                     || (State.CategoriesSelector.IsChecked && State.CategoriesSelector.Selected != null)
                     || (State.TypesSelector.IsChecked && State.TypesSelector.Selected != null)
                     || State.TagsSelector.IsChecked)
                   && SearchViewModel.MatchingTransactions.Any();
        }

        private void ExecutePerformCommand()
        {
            var transactions = SearchViewModel.MatchingTransactions;
            State.Execute(transactions, SearchViewModel.IsTransactionsSearch, SearchViewModel.MatchingPositions);
            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<Transaction[]>(transactions)));
        }

        public void Update()
        {
            State.Update(_queryDispatcher);
            SearchViewModel.Update();
        }
    }
}