using System.Collections.ObjectModel;
using System.Linq;

using AutoMapper;

using CashManager.Data.ViewModelState;
using CashManager.Features.Search;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Parsers;
using CashManager.Infrastructure.Command.ReplacerState;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Logic.Commands.Setters;
using CashManager.Logic.Parsers.Custom;
using CashManager.Model.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Transaction = CashManager.Data.DTO.Transaction;

namespace CashManager.Features.MassReplacer
{
    public class MassReplacerViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ISetter<Model.Transaction>[] _transactionSetters;
        private readonly ISetter<Model.Position>[] _positionSetter;

        public SearchViewModel SearchViewModel { get; private set; }

        public ReplacerState State { get; private set; }
        public ObservableCollection<BaseObservableObject> Patterns { get; private set; }

        public RelayCommand PerformCommand { get; }

        public RelayCommand<string> MassReplacerSaveCommand { get; }
        public RelayCommand<BaseObservableObject> MassReplacerLoadCommand { get; }

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
                SingleSetterCommand.Create(State.CategoriesSelector),
                SingleSetterCommand.Create(State.UserStocksSelector),
                SingleSetterCommand.Create(State.ExternalStocksSelector),
                SingleSetterCommand.Create(State.TypesSelector),
                MultiSetterCommand.Create(State.TagsSelector)
            };
            _positionSetter = _transactionSetters.OfType<ISetter<Model.Position>>().ToArray();

            MassReplacerSaveCommand = new RelayCommand<string>(name =>
            {
                State.Name = name;
                State.SearchState = SearchViewModel.State;
                var state = Mapper.Map<MassReplacerState>(State);
                _commandDispatcher.Execute(new UpsertReplacerStateCommand(state));

                Patterns.Remove(State);
                Patterns.Add(State);
            }, name => !string.IsNullOrWhiteSpace(name));
            MassReplacerLoadCommand = new RelayCommand<BaseObservableObject>(selected =>
            {
                State = new ReplacerState();

                var state = selected as ReplacerState;
                SearchViewModel.State.ApplySearchCriteria(state.SearchState);
                State.ApplyReplaceCriteria(state);
            }, selected => selected != null);
            Patterns = new ObservableCollection<BaseObservableObject>(); //todo: load
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