using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Filters;

using GalaSoft.MvvmLight;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoType = CashManager.Data.DTO.TransactionType;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionSearchViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private TrulyObservableCollection<Transaction> _matchingTransactions;
        private TimeFrame _bookDate = new TimeFrame("Book date");
        private TimeFrame _createDate = new TimeFrame("Create date");
        private TimeFrame _lastEditDate = new TimeFrame("Last edit date");
        private MultiPicker _userStocks;
        private MultiPicker _externalStocks;
        private MultiPicker _categories;
        private MultiPicker _types;
        private MultiPicker _tags;
        private RangeFilter _transactionValueFilter;

        public TransactionListViewModel TransactionsListViewModel { get; }

        public TrulyObservableCollection<Transaction> MatchingTransactions
        {
            get => _matchingTransactions;
            set => Set(nameof(MatchingTransactions), ref _matchingTransactions, value);
        }

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
        
        public TransactionSearchViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();

            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()));
            UserStocks = new MultiPicker("User stock", availableStocks.Where(x => x.IsUserStock).ToArray());
            ExternalStocks = new MultiPicker("External stock", Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks))); //we don't want to have same reference in 2 pickers
            
            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            Categories = new MultiPicker("Categories", categories);

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery()));
            Types = new MultiPicker("Types", types);

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()));
            Tags = new MultiPicker("Tags", tags);

            TransactionValueFilter = new RangeFilter("Transaction value");
        }
    }
}