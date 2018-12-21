using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Logic.Commands;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoType = CashManager.Data.DTO.TransactionType;
using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.Search
{
    public class SearchViewModel : ViewModelBase, IUpdateable
    {
        #region fields

        private readonly IQueryDispatcher _queryDispatcher;
        private Transaction[] _allTransactions;

        private Transaction[] _transactions;
        private Position[] _positions;

        private string _title;
        private bool _isTransactionsSearch;
        private bool _isPositionsSearch;
        
        private TrulyObservableCollection<IFilter<Transaction>> _transactionFilters;
        private TrulyObservableCollection<IFilter<Position>> _positionFilters;

        #endregion

        #region properties

        public TransactionListViewModel TransactionsListViewModel { get; }

        public PositionListViewModel PositionsListViewModel { get; }

        public SearchState SearchState { get; }

        public Transaction[] Transactions
        {
            get => _transactions;
            set => Set(nameof(Transactions), ref _transactions, value);
        }

        public Position[] Positions
        {
            get => _positions;
            set => Set(nameof(Positions), ref _positions, value);
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

        #endregion

        public SearchViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            SearchState = new SearchState();
            _queryDispatcher = queryDispatcher;
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();
            PositionsListViewModel = factory.Create<PositionListViewModel>();
            IsTransactionsSearch = true;
            Update();
        }

        public void Update()
        {
            _allTransactions = Mapper.Map<Transaction[]>(_queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()));
            Transactions = _allTransactions.ToArray();
            Positions = new Position[0];
            
            var availableStocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery())).OrderBy(x => x.Name);
            SearchState.UserStocksFilter = new MultiPicker(MultiPickerType.UserStock, availableStocks.Where(x => x.IsUserStock).ToArray());
            SearchState.ExternalStocksFilter =
                new MultiPicker(MultiPickerType.ExternalStock,
                    Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks))); //we don't want to have same reference in 2 pickers

            var categories = Mapper.Map<Category[]>(_queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories).ToArray();
            SearchState.CategoriesFilter = new MultiPicker(MultiPickerType.Category, categories);

            var types = Mapper.Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery())
                                                                     .OrderBy(x => x.Name));
            SearchState.TypesFilter = new MultiPicker(MultiPickerType.TransactionType, types);

            var tags = Mapper.Map<Tag[]>(_queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()).OrderBy(x => x.Name));
            SearchState.TagsFilter = new MultiPicker(MultiPickerType.Tag, tags);

            SearchState.ValueFilter = new RangeSelector(RangeSelectorType.GrossValue);

            var filters = new IFilter<Transaction>[]
            {
                DateFrameFilter.Create(SearchState.BookDateFilter),
                DateFrameFilter.Create(SearchState.CreateDateFilter),
                DateFrameFilter.Create(SearchState.LastEditDateFilter),
                TextFilter.Create(SearchState.TitleFilter),
                TextFilter.Create(SearchState.NoteFilter),
                TextFilter.Create(SearchState.PositionTitleFilter),
                MultiPickerFilter.Create(SearchState.CategoriesFilter),
                MultiPickerFilter.Create(SearchState.TagsFilter),
                MultiPickerFilter.Create(SearchState.TypesFilter),
                MultiPickerFilter.Create(SearchState.UserStocksFilter),
                MultiPickerFilter.Create(SearchState.ExternalStocksFilter),
                RangeFilter.Create(SearchState.ValueFilter)
            };
            //todo: do not create new list each time!
            _transactionFilters = new TrulyObservableCollection<IFilter<Transaction>>(filters);
            _positionFilters = new TrulyObservableCollection<IFilter<Position>>(filters.OfType<IFilter<Position>>());
            _transactionFilters.CollectionChanged += FiltersOnCollectionChanged;
            _positionFilters.CollectionChanged += FiltersOnCollectionChanged;

            FiltersOnCollectionChanged(this, null);
        }

        private void FiltersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (_allTransactions == null || !_allTransactions.Any()) return;

            if (IsTransactionsSearch)
            {
                if (!CanExecuteAnyTransactionFilter)
                    if (TransactionsListViewModel.Transactions.Count == _allTransactions.Length) return;

                FilterTransactions();
            }
            else if (IsPositionsSearch)
            {
                if (!CanExecuteAnyPositionFilter)
                    if (PositionsListViewModel.Positions.Count == _allTransactions.Sum(x => x.Positions.Count)) return;
                FilterPositions();
            }
        }

        private void SetTitle(SearchType searchType)
        {
            Title = $"{searchType} search";
        }

        private void FilterTransactions()
        {
            if (IsTransactionsSearch)
            {
                var input = _allTransactions.AsEnumerable();
                foreach (var filter in _transactionFilters)
                    if (filter.CanExecute())
                        input = filter.Execute(input);

                Transactions = input
                               .OrderByDescending(x => x.BookDate)
                               .ThenByDescending(x => x.InstanceCreationDate)
                               .ToArray();

                TransactionsListViewModel.Transactions.Clear();
                TransactionsListViewModel.Transactions.AddRange(Transactions);
            }
        }

        private void FilterPositions()
        {
            if (IsPositionsSearch)
            {
                var input = _allTransactions.SelectMany(x => x.Positions);
                foreach (var filter in _positionFilters)
                    if (filter.CanExecute())
                        input = filter.Execute(input);

                Positions = input
                               .OrderByDescending(x => x.BookDate)
                               .ThenByDescending(x => x.InstanceCreationDate)
                               .ToArray();

                PositionsListViewModel.Positions.Clear();
                PositionsListViewModel.Positions.AddRange(Positions);
            }
        }
    }
}