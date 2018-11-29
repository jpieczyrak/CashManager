using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Transaction
{
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly ViewModelFactory _factory;
        private IQueryDispatcher _queryDispatcher;

        public TrulyObservableCollection<Model.Transaction> Transactions { get; set; }

        public RelayCommand TransactionEditCommand => new RelayCommand(TransactionEdit, () => true);

        public Model.Transaction SelectedTransaction { get; set; }

        private void TransactionEdit()
        {
            var window = new TransactionView(SelectedTransaction, _factory.Create<TransactionViewModel>())
            {
                Title = SelectedTransaction?.Title ?? string.Empty
                //Left = mainWindow.Left + mainWindow.Width,
                //Top = mainWindow.Top
            };
            window.Show();
        }
        public TransactionListViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _factory = factory;
            var items = _queryDispatcher.Execute<TransactionQuery, CashManager.Data.DTO.Transaction[]>(new TransactionQuery());
            var transactions = items.Select(Mapper.Map<Model.Transaction>).ToArray();
            Transactions = new TrulyObservableCollection<Model.Transaction>(transactions);
        }
    }
}