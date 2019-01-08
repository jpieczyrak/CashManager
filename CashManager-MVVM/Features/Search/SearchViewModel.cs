using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.States;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.States;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Features.Transactions.Positions;
using CashManager_MVVM.Logic.Commands;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoSearchState = CashManager.Data.ViewModelState.SearchState;

namespace CashManager_MVVM.Features.Search
{
    public class SearchViewModel : ViewModelBase, IUpdateable
    {
        #region fields

        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly TransactionsProvider _transactionsProvider;

        private List<Transaction> _matchingTransactions;
        private List<Position> _matchingPositions;

        private string _title;
        private bool _isTransactionsSearch;
        private bool _isPositionsSearch;
        
        private readonly TrulyObservableCollection<IFilter<Transaction>> _transactionFilters;
        private readonly TrulyObservableCollection<IFilter<Position>> _positionFilters;
        private string _searchName;
        private BaseSelectable _selectedSearch;

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
                FiltersOnCollectionChanged(null, null);
            }
        }

        public bool IsPositionsSearch
        {
            get => _isPositionsSearch;
            set
            {
                Set(nameof(IsPositionsSearch), ref _isPositionsSearch, value);
                if (value) SetTitle(SearchType.Positions);
                FiltersOnCollectionChanged(null, null);
            }
        }

        private bool CanExecuteAnyPositionFilter => _positionFilters.Any(x => x.CanExecute());

        private bool CanExecuteAnyTransactionFilter => _transactionFilters.Any(x => x.CanExecute());

        public RelayCommand SaveSearch { get; set; }
        public RelayCommand LoadSearch { get; set; }

        public string SearchName
        {
            get => _searchName;
            set => Set(nameof(SearchName), ref _searchName, value);
        }

        public BaseSelectable SelectedSearch
        {
            get => _selectedSearch;
            set
            {
                Set(nameof(SelectedSearch), ref _selectedSearch, value);
                SearchName = SelectedSearch.Name;
            }
        }

        public BaseSelectable[] SaveSearches { get; set; }

        #endregion

        public SearchViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory, TransactionsProvider transactionsProvider)
        {
            State = new SearchState(queryDispatcher);
            State.PropertyChanged += (sender, args) => FiltersOnCollectionChanged(null, null);
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _transactionsProvider = transactionsProvider;

            SaveSearch = new RelayCommand(ExecuteSaveSearchStateCommand);
            LoadSearch = new RelayCommand(ExecuteLoadSearchStateCommand);
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();
            PositionsListViewModel = factory.Create<PositionListViewModel>();
            _isTransactionsSearch = true;
            SetTitle(SearchType.Transactions);
            _searchName = SearchState.DEFAULT_NAME;

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
            _transactionFilters.CollectionChanged += FiltersOnCollectionChanged;
            _positionFilters.CollectionChanged += FiltersOnCollectionChanged;
        }

        private void ExecuteLoadSearchStateCommand()
        {
            if (SelectedSearch == null) return;

            var query = new SearchStateQuery(x => x.Id == SelectedSearch.Id);
            var result = _queryDispatcher.Execute<SearchStateQuery, DtoSearchState[]>(query).FirstOrDefault();
            if (result != null)
                State.ApplySearchCriteria(Mapper.Map<SearchState>(result));
        }

        private void ExecuteSaveSearchStateCommand()
        {
            State.Name = SearchName;
            _commandDispatcher.Execute(new UpsertSearchStateCommand(Mapper.Map<DtoSearchState>(State)));
            SaveSearches = SaveSearches.Concat(new[] { new BaseSelectable(State.Id) { Name = State.Name } })
                                       .Distinct()
                                       .ToArray();
            RaisePropertyChanged(nameof(SaveSearches));
        }

        public void Update()
        {
            MatchingTransactions = _transactionsProvider.AllTransactions.ToList();
            MatchingPositions = new List<Position>();
            var states = _queryDispatcher.Execute<SearchStateQuery, DtoSearchState[]>(new SearchStateQuery());
            SaveSearches = states
                           .Select(x => new BaseSelectable(x.Id) { Name = x.Name })
                           .ToArray();
            State.UpdateSources(_queryDispatcher, _transactionsProvider);
            var defaultSearch = states.FirstOrDefault(x => x.Name == SearchState.DEFAULT_NAME);
            if (defaultSearch != null) State.ApplySearchCriteria(Mapper.Map<SearchState>(defaultSearch));
            
            FiltersOnCollectionChanged(this, null);
        }

        private void FiltersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var transactions = _transactionsProvider.AllTransactions;
            if (transactions == null || !transactions.Any()) return;

            if (IsTransactionsSearch)
            {
                if (!CanExecuteAnyTransactionFilter)
                    if (TransactionsListViewModel.Transactions.Count == transactions.Count) return;

                FilterTransactions();
            }
            else if (IsPositionsSearch)
            {
                if (!CanExecuteAnyPositionFilter)
                    if (PositionsListViewModel.Positions.Count == transactions.Sum(x => x.Positions.Count)) return;
                FilterPositions();
            }
        }

        private void SetTitle(SearchType searchType)
        {
            Title = $"{searchType} search";
        }

        private void FilterTransactions()
        {
            var input = _transactionsProvider.AllTransactions.AsEnumerable();
            foreach (var filter in _transactionFilters)
                if (filter.CanExecute())
                    input = filter.Execute(input);

            MatchingTransactions = OrderResults(input);
            TransactionsListViewModel.Transactions.Clear();
            TransactionsListViewModel.Transactions.AddRange(MatchingTransactions);
        }

        private void FilterPositions()
        {
            var input = _transactionsProvider.AllTransactions.SelectMany(x => x.Positions);
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