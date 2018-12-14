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
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoType = CashManager.Data.DTO.TransactionType;
using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionSearchViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private Transaction[] _allTransactions;
        private TextSelector _titleFilter = new TextSelector("Title");
        private TextSelector _noteFilter = new TextSelector("Note");
        private DateFrame _bookDateFilter = new DateFrame("Book date");
        private DateFrame _createDateFilter = new DateFrame("Create date");
        private DateFrame _lastEditDateFilter = new DateFrame("Last edit date");
        private MultiPicker _userStocksFilter;
        private MultiPicker _externalStocksFilter;
        private MultiPicker _categoriesFilter;
        private MultiPicker _typesFilter;
        private MultiPicker _tagsFilter;
        private RangeSelector _transactionValueFilter;
        private Transaction[] _transactions;

        public TransactionListViewModel TransactionsListViewModel { get; }

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

        public Transaction[] Transactions
        {
            get => _transactions;
            set => Set(nameof(Transactions), ref _transactions, value);
        }
        
        public TransactionSearchViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();
            Update();
        }

        public void Update()
        {
            _allTransactions = Mapper.Map<Transaction[]>(_queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()));
            Transactions = _allTransactions.ToArray();

            TitleFilter.PropertyChanged += OnPropertyChanged;
            NoteFilter.PropertyChanged += OnPropertyChanged;

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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
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
                transactions = transactions.Where(x => x.InstanceCreationDate >= CreateDateFilter.From && x.InstanceCreationDate <= CreateDateFilter.To);
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
                transactions = transactions.Where(x => x.ValueAsProfit >= TransactionValueFilter.Min && x.ValueAsProfit <= TransactionValueFilter.Max);
            }

            Transactions = transactions
                           .OrderByDescending(x => x.BookDate)
                           .ThenByDescending(x => x.InstanceCreationDate)
                           .ToArray();

            //Todo: change to dependency property binding. remove this:
            TransactionsListViewModel.Transactions.Clear();
            foreach (var transaction in Transactions) TransactionsListViewModel.Transactions.Add(transaction);
        }
    }
}