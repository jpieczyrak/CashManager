using CashManager_MVVM.Features.Transaction;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main
{
    public class MainViewModel : ViewModelBase
    {
        public TransactionListViewModel SelectedViewModel { get; set; }

        public MainViewModel(ViewModelFactory factory)
        {
            SelectedViewModel = factory.Create<TransactionListViewModel>();
        }
    }
}