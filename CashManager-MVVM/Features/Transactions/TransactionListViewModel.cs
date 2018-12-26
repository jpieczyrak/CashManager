using System.Collections.Specialized;
using System.Linq;

using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly ViewModelFactory _factory;
        private TrulyObservableCollection<Transaction> _transactions;

        public TrulyObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                if (_transactions != null) _transactions.CollectionChanged -= TransactionsOnCollectionChanged;
                _transactions = value;
                _transactions.CollectionChanged += TransactionsOnCollectionChanged;
                TransactionsOnCollectionChanged(this, null);
            }
        }

        public RelayCommand TransactionEditCommand => new RelayCommand(TransactionEdit, () => true);

        public Transaction SelectedTransaction { get; set; }

        public Summary Summary { get; set; }

        public TransactionListViewModel()
        {
            Summary = new Summary();
            Transactions = new TrulyObservableCollection<Transaction>();
        }

        public TransactionListViewModel(ViewModelFactory factory) : this()
        {
            _factory = factory;
        }
        
        private void TransactionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Summary.GrossIncome = Transactions.Where(x => x.Type.Income && !x.Type.Outcome).Sum(x => x.Value);
            Summary.GrossOutcome = Transactions.Where(x => !x.Type.Income && x.Type.Outcome).Sum(x => x.Value);
        }

        private void TransactionEdit()
        {
            var applicationViewModel = _factory.Create<ApplicationViewModel>();
            var transactionViewModel = _factory.Create<TransactionViewModel>();
            transactionViewModel.Transaction = SelectedTransaction;
            applicationViewModel.SetViewModelCommand.Execute(transactionViewModel);
        }
    }
}