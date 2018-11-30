using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Features.Stocks;
using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Features.TransactionTypes;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Main
{
    public class ApplicationViewModel : ViewModelBase
    {
        private ViewModelBase _selectedViewModel;

        public ViewModelBase SelectedViewModel
        {
            get => _selectedViewModel;
            private set => Set(ref _selectedViewModel, value, nameof(SelectedViewModel));
        }

        public Dictionary<string, ViewModelBase> ViewModels { get; private set; }

        public RelayCommand<ViewModelBase> SetViewModelCommand { get; private set; }

        public ApplicationViewModel(ViewModelFactory factory)
        {
            SetViewModelCommand = new RelayCommand<ViewModelBase>(view => SelectedViewModel = view);
            ViewModels = new Dictionary<string, ViewModelBase>
            {
                { "Transactions list", factory.Create<TransactionListViewModel>() },
                { "Stocks manager", factory.Create<StocksViewModel>() },
                { "Types manager", factory.Create<TransactionTypesViewModel>() },
                { "empty", null }
            };
            SelectedViewModel = ViewModels.FirstOrDefault().Value;
        }
    }
}