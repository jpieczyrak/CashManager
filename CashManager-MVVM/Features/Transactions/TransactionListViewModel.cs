using System;
using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewModelFactory _factory;
        private readonly TransactionsProvider _provider;
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

        public RelayCommand DuplicateTransactionCommand => new RelayCommand(TransactionDuplicate, CanExecuteTransactionDuplicate);

        public Transaction SelectedTransaction { get; set; }

        public TransactionsSummary Summary { get; set; }

        public TransactionListViewModel()
        {
            Summary = new TransactionsSummary();
            Transactions = new TrulyObservableCollection<Transaction>();
        }

        public TransactionListViewModel(ICommandDispatcher commandDispatcher, ViewModelFactory factory, TransactionsProvider provider) : this()
        {
            _commandDispatcher = commandDispatcher;
            _factory = factory;
            _provider = provider;
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

        private void TransactionDuplicate()
        {
            var transaction = Transaction.Copy(SelectedTransaction);
            var dto = Mapper.Map<CashManager.Data.DTO.Transaction>(transaction);

            _commandDispatcher.Execute(new UpsertTransactionsCommand(dto));
            _provider.AllTransactions.Add(transaction);
            Transactions.Add(transaction);
        }

        private bool CanExecuteTransactionDuplicate() => SelectedTransaction != null
                                                         && _provider != null
                                                         && _commandDispatcher != null;
    }
}