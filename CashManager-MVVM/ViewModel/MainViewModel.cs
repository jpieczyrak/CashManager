using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

using Logic.Model;
using Logic.Utils;

using Transaction = CashManager_MVVM.Model.Transaction;

namespace CashManager_MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public TrulyObservableCollection<Transaction> Transactions { get; set; }

        public MainViewModel(IDataService dataService)
        {
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