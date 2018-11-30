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
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ViewModelFactory _factory;

        public TrulyObservableCollection<Transaction> Transactions { get; set; } = new TrulyObservableCollection<Transaction>();

        public RelayCommand TransactionEditCommand => new RelayCommand(TransactionEdit, () => true);

        public Transaction SelectedTransaction { get; set; }

        public RelayCommand UpdateSourceCommand { get; set; }

        public TransactionListViewModel()
        {
            UpdateSourceCommand = new RelayCommand(LoadTransactionsFromDatabase);
        }

        public TransactionListViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory) : this()
        {
            _queryDispatcher = queryDispatcher;
            _factory = factory;
        }

        public void LoadTransactionsFromDatabase()
        {
            if (_queryDispatcher != null)
            {
                var items = _queryDispatcher.Execute<TransactionQuery, CashManager.Data.DTO.Transaction[]>(new TransactionQuery());
                var transactions = items.Select(Mapper.Map<Transaction>).ToArray();
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