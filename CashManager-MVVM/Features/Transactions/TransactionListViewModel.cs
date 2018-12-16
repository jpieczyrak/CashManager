using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;

using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionListViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
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

        public TransactionListViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory) : this()
        {
            _queryDispatcher = queryDispatcher;
            _factory = factory;
        }

        #region IUpdateable

        public void Update()
        {
            LoadTransactionsFromDatabase();
        }

        #endregion
        
        private void TransactionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Summary.GrossIncome = Transactions.Where(x => x.Type.Income && !x.Type.Outcome).Sum(x => x.Value);
            Summary.GrossOutcome = Transactions.Where(x => !x.Type.Income && x.Type.Outcome).Sum(x => x.Value);
        }

        private void LoadTransactionsFromDatabase()
        {
            if (_queryDispatcher != null)
            {
                var items = _queryDispatcher.Execute<TransactionQuery, CashManager.Data.DTO.Transaction[]>(new TransactionQuery());
                var transactions = items.Select(Mapper.Map<Transaction>)
                                        .OrderByDescending(x => x.BookDate)
                                        .ThenByDescending(x => x.InstanceCreationDate)
                                        .ToArray();
                Transactions = new TrulyObservableCollection<Transaction>(transactions);
            }
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