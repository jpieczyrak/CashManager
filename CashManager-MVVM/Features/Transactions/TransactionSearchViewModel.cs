using CashManager.Infrastructure.Query;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionSearchViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private TrulyObservableCollection<Transaction> _matchingTransactions;
        private TimeFrame _bookDate = new TimeFrame("Book date");
        private TimeFrame _createDate = new TimeFrame("Create date");
        private TimeFrame _lastEditDate = new TimeFrame("Last edit date");

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
        
        public TransactionSearchViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();
        }
    }
}