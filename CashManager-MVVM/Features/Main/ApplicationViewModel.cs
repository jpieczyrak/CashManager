using CashManager_MVVM.Features.Transaction;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main
{
    public class ApplicationViewModel : ViewModelBase
    {
        public TransactionListViewModel SelectedViewModel { get; set; }

        public ApplicationViewModel(ViewModelFactory factory)
        {
            SelectedViewModel = factory.Create<TransactionListViewModel>();
        }
    }
}