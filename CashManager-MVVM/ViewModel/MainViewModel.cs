using System.Windows;

using CashManager_MVVM.Model.DataProviders;

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
                var title = SelectedTransaction.Title;
                MessageBox.Show("there will be transaction edit window", title);
            });
            _dataService = dataService;
            _dataService.GetData(
                (transactions, error) =>
                {
                    if (error != null) return;
                    Transactions = transactions;
                });
        }
    }
}