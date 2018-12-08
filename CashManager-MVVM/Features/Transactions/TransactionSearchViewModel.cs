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

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Filters;

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
        private TextFilter _title = new TextFilter("Title");
        private TextFilter _note = new TextFilter("Note");
        private TimeFrame _bookDate = new TimeFrame("Book date");
        private TimeFrame _createDate = new TimeFrame("Create date");
        private TimeFrame _lastEditDate = new TimeFrame("Last edit date");
        private MultiPicker _userStocks;
        private MultiPicker _externalStocks;
        private MultiPicker _categories;
        private MultiPicker _types;
        private MultiPicker _tags;
        private RangeFilter _transactionValueFilter;
        private Transaction[] _transactions;

        public TransactionListViewModel TransactionsListViewModel { get; }

        public TimeFrame BookDate
        {
            get => _bookDate;
            set => Set(nameof(BookDate), ref _bookDate, value);
        }

        public TimeFrame CreateDate
        {
            get => _createDate;
            set => Set(nameof(CreateDate), ref _createDate, value);
        }

        public TimeFrame LastEditDate
        {
            get => _lastEditDate;
            set => Set(nameof(LastEditDate), ref _lastEditDate, value);
        }

        public MultiPicker UserStocks
        {
            get => _userStocks;
            set => Set(nameof(UserStocks), ref _userStocks, value);
        }

        public MultiPicker ExternalStocks
        {
            get => _externalStocks;
            set => Set(nameof(ExternalStocks), ref _externalStocks, value);
        }

        public MultiPicker Categories
        {
            get => _categories;
            set => Set(nameof(Categories), ref _categories, value);
        }

        public MultiPicker Types
        {
            get => _types;
            set => Set(nameof(Types), ref _types, value);
        }

        public MultiPicker Tags
        {
            get => _tags;
            set => Set(nameof(Tags), ref _tags, value);
        }

        public RangeFilter TransactionValueFilter
        {
            get => _transactionValueFilter;
            set => Set(nameof(TransactionValueFilter), ref _transactionValueFilter, value);
        }

        public TextFilter Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        public TextFilter Note
        {
            get => _note;
            set => Set(nameof(Note), ref _note, value);
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

            Title.PropertyChanged += OnPropertyChanged;
            Note.PropertyChanged += OnPropertyChanged;

            BookDate.PropertyChanged += OnPropertyChanged;
            LastEditDate.PropertyChanged += OnPropertyChanged;
            CreateDate.PropertyChanged += OnPropertyChanged;

            var availableStocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocks = new MultiPicker("User stock", availableStocks.Where(x => x.IsUserStock).ToArray());
            ExternalStocks =
                new MultiPicker("External stock",
                    Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks))); //we don't want to have same reference in 2 pickers
            UserStocks.PropertyChanged += OnPropertyChanged;
            ExternalStocks.PropertyChanged += OnPropertyChanged;

            var categories = Mapper.Map<Category[]>(_queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            categories = BuildGraphicalOrder(categories).ToArray();
            Categories = new MultiPicker("Categories", categories);
            Categories.PropertyChanged += OnPropertyChanged;

            var types = Mapper.Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery())
                                                                     .OrderBy(x => x.Name));
            Types = new MultiPicker("Types", types);
            Types.PropertyChanged += OnPropertyChanged;

            var tags = Mapper.Map<Tag[]>(_queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()).OrderBy(x => x.Name));
            Tags = new MultiPicker("Tags", tags);
            Tags.PropertyChanged += OnPropertyChanged;

            TransactionValueFilter = new RangeFilter("Transaction value");
            TransactionValueFilter.PropertyChanged += OnPropertyChanged;

            OnPropertyChanged(this, null);
        }

        private List<Category> BuildGraphicalOrder(Category[] categories, Category root = null, int index = 0)
        {
            var results = new List<Category>();
            if (root != null) results.Add(root);
            var children = categories.Where(x => Equals(x.Parent, root)).ToArray();
            foreach (var category in children)
            {
                category.Name = $"{string.Join(string.Empty, Enumerable.Range(0, index).Select(x => " "))}{category.Name}";
                results.AddRange(BuildGraphicalOrder(categories, category, index + 1));
            }

            return results;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var transactions = _allTransactions.AsEnumerable();
            if (Title.IsChecked)
            {
                if (Note.IsChecked)
                    transactions = transactions.Where(x =>
                        x.Title.ToLower().Contains(Title.Value.ToLower()) || x.Note.ToLower().Contains(Note.Value.ToLower()));
                else transactions = transactions.Where(x => x.Title.ToLower().Contains(Title.Value.ToLower()));
            }
            else if (Note.IsChecked) transactions = transactions.Where(x => x.Note.ToLower().Contains(Note.Value.ToLower()));

            if (Categories.IsChecked)
            {
                var categories = Categories.Results.OfType<Category>().ToArray();
                transactions = transactions.Where(x => x.Positions.Select(y => y.Category).Any(y => categories.Any(z =>
                    z.MatchCategoryFilter(y))));
            }

            if (Tags.IsChecked)
            {
                var tags = new HashSet<Tag>(Tags.Results.OfType<Tag>());
                transactions = transactions.Where(x => x.Positions.SelectMany(y => y.Tags).Any(y => tags.Contains(y)));
            }
            
            if (Types.IsChecked)
            {
                var types = new HashSet<TransactionType>(Types.Results.OfType<TransactionType>());
                transactions = transactions.Where(x => types.Contains(x.Type));
            }

            if (UserStocks.IsChecked)
            {
                var stocks = new HashSet<Stock>(UserStocks.Results.OfType<Stock>());
                transactions = transactions.Where(x => stocks.Contains(x.UserStock));
            }

            if (ExternalStocks.IsChecked)
            {
                var stocks = new HashSet<Stock>(ExternalStocks.Results.OfType<Stock>());
                transactions = transactions.Where(x => stocks.Contains(x.ExternalStock));
            }

            if (CreateDate.IsChecked)
            {
                transactions = transactions.Where(x => x.InstanceCreationDate >= CreateDate.From && x.InstanceCreationDate <= CreateDate.To);
            }

            if (BookDate.IsChecked)
            {
                transactions = transactions.Where(x => x.BookDate >= BookDate.From && x.BookDate <= BookDate.To);
            }

            if (LastEditDate.IsChecked)
            {
                transactions = transactions.Where(x => x.LastEditDate >= LastEditDate.From && x.LastEditDate <= LastEditDate.To);
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