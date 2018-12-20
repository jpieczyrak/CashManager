using System.Collections.Generic;
using System.ComponentModel;
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
        private TextSelector _titleFilter = new TextSelector("Title");
        private TextSelector _noteFilter = new TextSelector("Note");
        private TextSelector _positionTitleFilter = new TextSelector("Position title");
        private DateFrame _bookDateFilter = DateFrame.Create(DateFrameType.BookDate);
        private DateFrame _createDateFilter = DateFrame.Create(DateFrameType.CreationDate);
        private DateFrame _lastEditDateFilter = DateFrame.Create(DateFrameType.EditDate);
        private MultiPicker _userStocksFilter;
        private MultiPicker _externalStocksFilter;
        private MultiPicker _categoriesFilter;
        private MultiPicker _typesFilter;
        private MultiPicker _tagsFilter;
        private RangeSelector _transactionValueFilter;
        private Transaction[] _transactions;
        private Position[] _positions;
        private string _title;
        private bool _isTransactionsSearch;
        private bool _isPositionsSearch;

        #endregion

        #region properties
        
        public TransactionListViewModel TransactionsListViewModel { get; }
        public PositionListViewModel PositionsListViewModel { get; }

        public DateFrame BookDateFilter
        {
            get => _bookDateFilter;
            set => Set(nameof(BookDateFilter), ref _bookDateFilter, value);
        }

        public DateFrame CreateDateFilter
        {
            get => _createDateFilter;
            set => Set(nameof(CreateDateFilter), ref _createDateFilter, value);
        }

        public DateFrame LastEditDateFilter
        {
            get => _lastEditDateFilter;
            set => Set(nameof(LastEditDateFilter), ref _lastEditDateFilter, value);
        }

        public MultiPicker UserStocksFilter
        {
            get => _userStocksFilter;
            set => Set(nameof(UserStocksFilter), ref _userStocksFilter, value);
        }

        public MultiPicker ExternalStocksFilter
        {
            get => _externalStocksFilter;
            set => Set(nameof(ExternalStocksFilter), ref _externalStocksFilter, value);
        }

        public MultiPicker CategoriesFilter
        {
            get => _categoriesFilter;
            set => Set(nameof(CategoriesFilter), ref _categoriesFilter, value);
        }

        public MultiPicker TypesFilter
        {
            get => _typesFilter;
            set => Set(nameof(TypesFilter), ref _typesFilter, value);
        }

        public MultiPicker TagsFilter
        {
            get => _tagsFilter;
            set => Set(nameof(TagsFilter), ref _tagsFilter, value);
        }

        public RangeSelector TransactionValueFilter
        {
            get => _transactionValueFilter;
            set => Set(nameof(TransactionValueFilter), ref _transactionValueFilter, value);
        }

        public TextSelector TitleFilter
        {
            get => _titleFilter;
            set => Set(nameof(TitleFilter), ref _titleFilter, value);
        }

        public TextSelector NoteFilter
        {
            get => _noteFilter;
            set => Set(nameof(NoteFilter), ref _noteFilter, value);
        }

        public TextSelector PositionTitleFilter
        {
            get => _positionTitleFilter;
            set => Set(nameof(PositionTitleFilter), ref _positionTitleFilter, value);
        }

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
                OnPropertyChanged(null, null);
            }
        }

        public bool IsPositionsSearch
        {
            get => _isPositionsSearch;
            set
            {
                Set(nameof(IsPositionsSearch), ref _isPositionsSearch, value);
                if (value) SetTitle(SearchType.Positions);
                OnPropertyChanged(null, null);
            }
        }

        public bool IsAnySelected => _bookDateFilter.IsChecked
                                     || _userStocksFilter.IsChecked && _userStocksFilter.Results.Any()
                                     || _externalStocksFilter.IsChecked && _externalStocksFilter.Results.Any()
                                     || _titleFilter.IsChecked && !string.IsNullOrWhiteSpace(_titleFilter.Value)
                                     || _noteFilter.IsChecked
                                     || _positionTitleFilter.IsChecked && !string.IsNullOrWhiteSpace(_positionTitleFilter.Value)
                                     || _categoriesFilter.IsChecked && _categoriesFilter.Results.Any()
                                     || _typesFilter.IsChecked && _typesFilter.Results.Any()
                                     || _tagsFilter.IsChecked;

        #endregion

        public SearchViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
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

            TitleFilter.PropertyChanged += OnPropertyChanged;
            NoteFilter.PropertyChanged += OnPropertyChanged;
            PositionTitleFilter.PropertyChanged += OnPropertyChanged;

            BookDateFilter.PropertyChanged += OnPropertyChanged;
            LastEditDateFilter.PropertyChanged += OnPropertyChanged;
            CreateDateFilter.PropertyChanged += OnPropertyChanged;

            var availableStocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksFilter = new MultiPicker("User stock", availableStocks.Where(x => x.IsUserStock).ToArray());
            ExternalStocksFilter =
                new MultiPicker("External stock",
                    Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks))); //we don't want to have same reference in 2 pickers
            UserStocksFilter.PropertyChanged += OnPropertyChanged;
            ExternalStocksFilter.PropertyChanged += OnPropertyChanged;

            var categories = Mapper.Map<Category[]>(_queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories).ToArray();
            CategoriesFilter = new MultiPicker("Categories", categories);
            CategoriesFilter.PropertyChanged += OnPropertyChanged;

            var types = Mapper.Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery())
                                                                     .OrderBy(x => x.Name));
            TypesFilter = new MultiPicker("Types", types);
            TypesFilter.PropertyChanged += OnPropertyChanged;

            var tags = Mapper.Map<Tag[]>(_queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsFilter = new MultiPicker("Tags", tags);
            TagsFilter.PropertyChanged += OnPropertyChanged;

            TransactionValueFilter = new RangeSelector("Transaction value");
            TransactionValueFilter.PropertyChanged += OnPropertyChanged;
            
            OnPropertyChanged(this, null);
        }

        private void SetTitle(SearchType searchType)
        {
            Title = $"{searchType} search";
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_allTransactions == null || !_allTransactions.Any()) return;
            if (!IsAnySelected)
            {
                if (IsTransactionsSearch)
                {
                    if (TransactionsListViewModel.Transactions.Count == _allTransactions.Length) return;
                }
                else if (IsPositionsSearch)
                    if (PositionsListViewModel.Positions.Count == _allTransactions.Sum(x => x.Positions.Count)) return;
            }

            if (IsTransactionsSearch) FilterTransactions();
            else if (IsPositionsSearch) FilterPositions();
        }

        #region filtering

        private void FilterTransactions()
        {
            GetFilteredTransactions();

            //Todo: change to dependency property binding. remove this:
            TransactionsListViewModel.Transactions.Clear();
            TransactionsListViewModel.Transactions.AddRange(Transactions);
        }

        private Transaction[] GetFilteredTransactions()
        {
            var transactions = _allTransactions.AsEnumerable();
            if (TitleFilter.IsChecked)
            {
                transactions = transactions.Where(x => x.Title.ToLower().Contains(TitleFilter.Value.ToLower()));
            }

            if (NoteFilter.IsChecked)
            {
                transactions = transactions.Where(x => !string.IsNullOrEmpty(x.Note) && x.Note.ToLower().Contains(NoteFilter.Value.ToLower()));
            }

            if (PositionTitleFilter.IsChecked)
            {
                transactions = transactions.Where(x => x.Positions.Any(y => y.Title.ToLower().Contains(PositionTitleFilter.Value.ToLower())));
            }

            if (CategoriesFilter.IsChecked)
            {
                var categories = CategoriesFilter.Results.OfType<Category>().ToArray();
                transactions = transactions.Where(x => x.Positions.Select(y => y.Category).Any(y => categories.Any(z =>
                    z.MatchCategoryFilter(y))));
            }

            if (TagsFilter.IsChecked)
            {
                var tags = new HashSet<Tag>(TagsFilter.Results.OfType<Tag>());
                transactions = transactions.Where(x => x.Positions.SelectMany(y => y.Tags).Any(y => tags.Contains(y)));
            }

            if (TypesFilter.IsChecked)
            {
                var types = new HashSet<TransactionType>(TypesFilter.Results.OfType<TransactionType>());
                transactions = transactions.Where(x => types.Contains(x.Type));
            }

            if (UserStocksFilter.IsChecked)
            {
                var stocks = new HashSet<Stock>(UserStocksFilter.Results.OfType<Stock>());
                transactions = transactions.Where(x => stocks.Contains(x.UserStock));
            }

            if (ExternalStocksFilter.IsChecked)
            {
                var stocks = new HashSet<Stock>(ExternalStocksFilter.Results.OfType<Stock>());
                transactions = transactions.Where(x => stocks.Contains(x.ExternalStock));
            }

            if (CreateDateFilter.IsChecked)
            {
                transactions = transactions.Where(x =>
                    x.InstanceCreationDate >= CreateDateFilter.From && x.InstanceCreationDate <= CreateDateFilter.To);
            }

            if (BookDateFilter.IsChecked)
            {
                transactions = transactions.Where(x => x.BookDate >= BookDateFilter.From && x.BookDate <= BookDateFilter.To);
            }

            if (LastEditDateFilter.IsChecked)
            {
                transactions = transactions.Where(x => x.LastEditDate >= LastEditDateFilter.From && x.LastEditDate <= LastEditDateFilter.To);
            }

            if (TransactionValueFilter.IsChecked)
            {
                transactions = transactions.Where(x =>
                    x.ValueAsProfit >= TransactionValueFilter.Min && x.ValueAsProfit <= TransactionValueFilter.Max);
            }

            Transactions = transactions
                           .OrderByDescending(x => x.BookDate)
                           .ThenByDescending(x => x.InstanceCreationDate)
                           .ToArray();

            return Transactions;
        }

        private void FilterPositions()
        {
            var transactions = GetFilteredTransactions();
            IEnumerable<Position> positions = transactions.SelectMany(x => x.Positions).ToArray();
            
            if (PositionTitleFilter.IsChecked)
            {
                positions = positions.Where(x => x.Title.ToLower().Contains(PositionTitleFilter.Value.ToLower()));
            }

            if (CategoriesFilter.IsChecked && CategoriesFilter.Results.Any())
            {
                var categories = CategoriesFilter.Results.OfType<Category>().ToArray();
                positions = positions.Where(x => categories.Any(y => y.MatchCategoryFilter(x.Category)));
            }

            if (TagsFilter.IsChecked)
            {
                var tags = new HashSet<Tag>(TagsFilter.Results.OfType<Tag>());
                positions = positions.Where(x => x.Tags.Any(y => tags.Contains(y)));
            }

            if (TransactionValueFilter.IsChecked)
            {
                positions = positions.Where(x => x.Value.GrossValue >= TransactionValueFilter.Min && x.Value.GrossValue <= TransactionValueFilter.Max);
            }

            Positions = positions.OrderByDescending(x => x.Parent.BookDate).ToArray();

            PositionsListViewModel.Positions.Clear();
            PositionsListViewModel.Positions.AddRange(Positions);
        }

        #endregion
    }
}