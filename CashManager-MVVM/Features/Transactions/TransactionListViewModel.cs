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

        public TransactionsSummary Summary { get; set; }

        public TransactionListViewModel()
        {
            Summary = new TransactionsSummary();
            Transactions = new TrulyObservableCollection<Transaction>();
        }

        public TransactionListViewModel(ViewModelFactory factory) : this()
        {
            _factory = factory;
        }
        
        private void TransactionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var incomes = Transactions.Where(x => x.Type.Income && !x.Type.Outcome).ToArray();
            var outcomes = Transactions.Where(x => !x.Type.Income && x.Type.Outcome).ToArray();
            Summary.GrossIncome = incomes.Sum(x => x.Value);
            Summary.GrossOutcome = outcomes.Sum(x => x.Value);
            Summary.IncomesCount = incomes.Length;
            Summary.OutcomesCount = outcomes.Length;
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