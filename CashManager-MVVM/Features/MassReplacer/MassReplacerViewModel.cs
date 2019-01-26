using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;

using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Logic.Commands.Setters;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Transaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.MassReplacer
{
    public class MassReplacerViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ISetter<Model.Transaction>[] _transactionSetters;
        private readonly ISetter<Model.Position>[] _positionSetter;

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

            _transactionSetters = new ISetter<Model.Transaction>[]
            {
                TextSetterCommand.Create(State.TitleSelector, SearchViewModel.State.TitleFilter),
                TextSetterCommand.Create(State.NoteSelector, SearchViewModel.State.NoteFilter),
                TextSetterCommand.Create(State.PositionTitleSelector, SearchViewModel.State.PositionTitleFilter),
                DateSetterCommand.Create(State.BookDateSetter),
                SinglePickerSetterCommand.Create(State.CategoriesSelector),
                SinglePickerSetterCommand.Create(State.UserStocksSelector),
                SinglePickerSetterCommand.Create(State.ExternalStocksSelector),
                SinglePickerSetterCommand.Create(State.TypesSelector),
                MultiSetterCommand.Create(State.TagsSelector)
            };
            _positionSetter = _transactionSetters.OfType<ISetter<Model.Position>>().ToArray();
        }

        private bool CanExecutePerformCommand()
        {
            return SearchViewModel.MatchingTransactions.Any() && _transactionSetters.Any(x => x.CanExecute());
        }

        private void ExecutePerformCommand()
        {
            var transactions = SearchViewModel.MatchingTransactions;

            if (SearchViewModel.IsTransactionsSearch)
            {
                foreach (var setter in _transactionSetters)
                    if (setter.CanExecute())
                        setter.Execute(transactions);
            }
            else
            {
                foreach (var setter in _positionSetter)
                    if (setter.CanExecute())
                        setter.Execute(SearchViewModel.MatchingPositions);
            }
            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<Transaction[]>(transactions)));
        }

        public void Update()
        {
            State.Update(_queryDispatcher);
            SearchViewModel.Update();
        }
    }
}