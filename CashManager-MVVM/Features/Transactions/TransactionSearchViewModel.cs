using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Filters;

using GalaSoft.MvvmLight;

using DtoStock = CashManager.Data.DTO.Stock;

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
        
        public TransactionSearchViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();

            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()));
            UserStocks = new MultiPicker("User stock", availableStocks.Where(x => x.IsUserStock).ToArray());
        }
    }
}