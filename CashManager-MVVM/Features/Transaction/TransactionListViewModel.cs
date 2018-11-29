using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;

using CashManager_MVVM.Features.Main;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Transaction
{
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly ViewModelFactory _factory;

        public TrulyObservableCollection<Model.Transaction> Transactions { get; set; }

        public RelayCommand TransactionEditCommand => new RelayCommand(TransactionEdit, () => true);

        public Model.Transaction SelectedTransaction { get; set; }

        private void TransactionEdit()
        {
            var applicationViewModel = _factory.Create<ApplicationViewModel>();
            var transactionViewModel = _factory.Create<TransactionViewModel>();
            transactionViewModel.Transaction = SelectedTransaction;
            applicationViewModel.SetViewModelCommand.Execute(transactionViewModel);
        }

        public TransactionListViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _factory = factory;
            var items = queryDispatcher.Execute<TransactionQuery, CashManager.Data.DTO.Transaction[]>(new TransactionQuery());
            var transactions = items.Select(Mapper.Map<Model.Transaction>).ToArray();
            Transactions = new TrulyObservableCollection<Model.Transaction>(transactions);
        }
    }
}