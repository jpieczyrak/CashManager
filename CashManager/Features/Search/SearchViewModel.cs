using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Features.Transactions;
using CashManager.Features.Transactions.Positions;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.States;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.States;
using CashManager.Logic;
using CashManager.Logic.Commands.Filters;
using CashManager.Logic.Wrappers;
using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Properties;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

using log4net;

using DtoSearchState = CashManager.Data.ViewModelState.SearchState;

namespace CashManager.Features.Search
{
    public class SearchViewModel : ViewModelBase, IUpdateable
    {
        #region fields

        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(SearchViewModel)));

        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        private List<Transaction> _matchingTransactions;
        private List<Position> _matchingPositions;

        private string _title;
        private bool _isTransactionsSearch;
        private bool _isPositionsSearch;

        private readonly TrulyObservableCollection<IFilter<Transaction>> _transactionFilters;
        private readonly TrulyObservableCollection<IFilter<Position>> _positionFilters;
        private readonly Debouncer _debouncer;

        #endregion

        #region properties

        public TransactionListViewModel TransactionsListViewModel { get; }

        public PositionListViewModel PositionsListViewModel { get; }

        public SearchState State { get; }

        public List<Transaction> MatchingTransactions
        {
            get => _matchingTransactions;
            set => Set(nameof(MatchingTransactions), ref _matchingTransactions, value);
        }

        public List<Position> MatchingPositions
        {
            get => _matchingPositions;
            set => Set(nameof(MatchingPositions), ref _matchingPositions, value);
        }

        public string Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        public bool IsTransactionsSearch
        {
            get => _isTransactionsSearch;
            set
            {
                Set(nameof(IsTransactionsSearch), ref _isTransactionsSearch, value);
                if (value) SetTitle(SearchType.Transactions);
                ScheduleFiltering(null, null);
            }
        }

        public bool IsPositionsSearch
        {
            get => _isPositionsSearch;
            set
            {
                Set(nameof(IsPositionsSearch), ref _isPositionsSearch, value);
                if (value) SetTitle(SearchType.Positions);
                ScheduleFiltering(null, null);
            }
        }

        public TransactionsProvider Provider { get; }

        private bool CanExecuteAnyPositionFilter => _positionFilters.Any(x => x.CanExecute());

        private bool CanExecuteAnyTransactionFilter => _transactionFilters.Any(x => x.CanExecute());

        public RelayCommand<string> SaveStateCommand { get; set; }
        public RelayCommand<BaseObservableObject> LoadStateCommand { get; set; }
        public RelayCommand ClearStateCommand { get; set; }

        public ObservableCollection<BaseObservableObject> SaveSearches { get; set; }

        public bool IsDebounceable { private get; set; } = true;

        #endregion

        public SearchViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory, TransactionsProvider transactionsProvider)
        {
            _debouncer = new Debouncer();
            State = new SearchState(queryDispatcher);
            State.PropertyChanged += (sender, args) => ScheduleFiltering(null, null);
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            Provider = transactionsProvider;

            SaveStateCommand = new RelayCommand<string>(ExecuteSaveSearchStateCommand);
            LoadStateCommand = new RelayCommand<BaseObservableObject>(ExecuteLoadSearchStateCommand);
            ClearStateCommand = new RelayCommand(() => State.Clear());
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();
            PositionsListViewModel = factory.Create<PositionListViewModel>();
            _isTransactionsSearch = true;
            SetTitle(SearchType.Transactions);

            var filters = new IFilter<Transaction>[]
            {
                DateFrameFilter.Create(State.BookDateFilter),
                DateFrameFilter.Create(State.CreateDateFilter),
                DateFrameFilter.Create(State.LastEditDateFilter),
                TextFilter.Create(State.TitleFilter),
                TextFilter.Create(State.NoteFilter),
                TextFilter.Create(State.PositionTitleFilter),
                MultiPickerFilter.Create(State.CategoriesFilter),
                MultiPickerFilter.Create(State.TagsFilter),
                MultiPickerFilter.Create(State.TypesFilter),
                MultiPickerFilter.Create(State.UserStocksFilter),
                MultiPickerFilter.Create(State.ExternalStocksFilter),
                RangeFilter.Create(State.ValueFilter)
            };
            _transactionFilters = new TrulyObservableCollection<IFilter<Transaction>>(filters);
            _positionFilters = new TrulyObservableCollection<IFilter<Position>>(filters.OfType<IFilter<Position>>());
            _transactionFilters.CollectionChanged += ScheduleFiltering;
            _positionFilters.CollectionChanged += ScheduleFiltering;
        }

        private void ScheduleFiltering(object o, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (IsDebounceable)
            {
                _logger.Value.Debug("Calling filter");
                _debouncer.Debouce(() => DispatcherHelper.RunAsync(() =>
                {
                    using (new MeasureTimeWrapper(() => PerformFilter(Provider.AllTransactions), "Running filter")) { }
                    _logger.Value.Debug("Running filter");
                }));
            }
            else
                PerformFilter(Provider.AllTransactions); //mainly for tests purpose
        }

        private void ExecuteLoadSearchStateCommand(BaseObservableObject selected)
        {
            var model = selected as SearchState;
            State.ApplySearchCriteria(model);
        }

        private void ExecuteSaveSearchStateCommand(string name)
        {
            State.Name = name;
            var state = Mapper.Map<DtoSearchState>(State);
            _commandDispatcher.Execute(new UpsertSearchStateCommand(state));
            var model = Mapper.Map<SearchState>(state);
            SaveSearches.Remove(model);
            SaveSearches.Add(model);
        }

        public void Update()
        {
            MatchingTransactions = Provider.AllTransactions.ToList();
            MatchingPositions = new List<Position>();
            var states = Mapper.Map<SearchState[]>(_queryDispatcher.Execute<SearchStateQuery, DtoSearchState[]>(new SearchStateQuery()));
            SaveSearches = new ObservableCollection<BaseObservableObject>(states);
            State.UpdateSources(_queryDispatcher, Provider);

            PerformFilter(Provider.AllTransactions);
        }

        public void PerformFilter(IEnumerable<Transaction> transactions)
        {
            if (transactions == null || !transactions.Any()) return;
            if (IsTransactionsSearch)
            {
                //todo: rethink - it could improve performance, but it fails refreshing grids after transaction edit cancel
                //if (!CanExecuteAnyTransactionFilter)if (TransactionsListViewModel.Transactions.Count == transactions.Count) return;

                FilterTransactions(transactions);
            }
            else if (IsPositionsSearch)
            {
                //if (!CanExecuteAnyPositionFilter) if (PositionsListViewModel.Positions.Count == transactions.Sum(x => x.Positions.Count)) return;
                FilterPositions(transactions.SelectMany(x => x.Positions));
            }
        }

        private void SetTitle(SearchType searchType)
        {
            Title = searchType == SearchType.Transactions ? Strings.TransactionSearch : Strings.PositionSearch;
        }

        private void FilterTransactions(IEnumerable<Transaction> transactions)
        {
            var input = transactions;
            foreach (var filter in _transactionFilters)
                if (filter.CanExecute())
                    input = filter.Execute(input);

            MatchingTransactions = OrderResults(input);
            TransactionsListViewModel.Transactions.Clear();
            TransactionsListViewModel.Transactions.AddRange(MatchingTransactions);
        }

        private void FilterPositions(IEnumerable<Position> positions)
        {
            var input = positions;
            foreach (var filter in _positionFilters)
                if (filter.CanExecute())
                    input = filter.Execute(input);

            MatchingPositions = OrderResults(input);
            PositionsListViewModel.Positions.Clear();
            PositionsListViewModel.Positions.AddRange(MatchingPositions);
        }

        private static List<T> OrderResults<T>(IEnumerable<T> input) where T : IBookable
        {
            return input
                   .OrderByDescending(x => x.BookDate)
                   .ThenByDescending(x => x.InstanceCreationDate)
                   .ToList();
        }
    }
}