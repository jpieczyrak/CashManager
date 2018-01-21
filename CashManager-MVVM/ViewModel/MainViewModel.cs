using CashManager_MVVM.Model.DataProviders;
using CashManager_MVVM.View;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Logic.Utils;

using Transaction = CashManager_MVVM.Model.Transaction;

namespace CashManager_MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public TrulyObservableCollection<Transaction> Transactions { get; set; }

        public RelayCommand TransactionEditCommand { get; set; }

        public Transaction SelectedTransaction { get; set; }

        public MainViewModel(IDataService dataService)
        {
            TransactionEditCommand = new RelayCommand(() =>
            {
                new TransactionView(SelectedTransaction) { Title = SelectedTransaction?.Title ?? string.Empty }.Show();
            });
            _dataService = dataService;
            _dataService.GetTransactions(
                (transactions, error) =>
                {
                    if (error != null) return;
                    Transactions = new TrulyObservableCollection<Transaction>(transactions);
                });
        }
    }
}