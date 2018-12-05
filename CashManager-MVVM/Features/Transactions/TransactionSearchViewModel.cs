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
    public class TransactionSearchViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly Transaction[] _allTransactions;
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
            _allTransactions = Mapper.Map<Transaction[]>(queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()));
            Transactions = _allTransactions.ToArray();

            TransactionsListViewModel = factory.Create<TransactionListViewModel>();

            Title.PropertyChanged += OnPropertyChanged;
            Note.PropertyChanged += OnPropertyChanged;

            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()));
            UserStocks = new MultiPicker("User stock", availableStocks.Where(x => x.IsUserStock).ToArray());
            ExternalStocks = new MultiPicker("External stock", Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks))); //we don't want to have same reference in 2 pickers
            
            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            Categories = new MultiPicker("Categories", categories);

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery()));
            Types = new MultiPicker("Types", types);
            Types.PropertyChanged += OnPropertyChanged;

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()));
            Tags = new MultiPicker("Tags", tags);
            Tags.PropertyChanged += OnPropertyChanged;

            TransactionValueFilter = new RangeFilter("Transaction value");
            OnPropertyChanged(this, null);
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

            //todo: categories

            if (Tags.IsChecked)
            {
                var tags = new HashSet<Tag>(Tags.Results.OfType<Tag>());
                transactions = transactions.Where(x => x.Positions.SelectMany(y => y.Tags).Any(y => tags.Contains(y)));
            }

            //todo: fix if Results is null or empty
            if (Types.IsChecked)
            {
                var types = new HashSet<TransactionType>(Types.Results.OfType<TransactionType>());
                transactions = transactions.Where(x => types.Contains(x.Type));
            }

            Transactions = transactions.ToArray();

            //Todo: change to binding. remove this:
            TransactionsListViewModel.Transactions.Clear();
            foreach (var transaction in Transactions)
            {
                TransactionsListViewModel.Transactions.Add(transaction);
            }
        }
    }
}