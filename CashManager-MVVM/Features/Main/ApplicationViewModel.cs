using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.Parsers;
using CashManager_MVVM.Features.Stocks;
using CashManager_MVVM.Features.Tags;
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
            private set
            {
                Set(ref _selectedViewModel, value, nameof(SelectedViewModel));
                if (_selectedViewModel is IUpdateable model) model.Update();
            }
        }

        public StockSummaryViewModel SummaryViewModel { get; }
        public Dictionary<string, ViewModelBase> ViewModels { get; private set; }

        public RelayCommand<ViewModelBase> SetViewModelCommand { get; private set; }

        public ApplicationViewModel(ViewModelFactory factory)
        {
            SetViewModelCommand = new RelayCommand<ViewModelBase>(view => SelectedViewModel = view);
            ViewModels = new Dictionary<string, ViewModelBase>
            {
                { "Transactions list", factory.Create<TransactionListViewModel>() },
                { "Add transaction", factory.Create<TransactionViewModel>() },
                { "Stocks manager", factory.Create<StocksViewModel>() },
                { "Category manager", factory.Create<CategoryManagerViewModel>() },
                { "Types manager", factory.Create<TransactionTypesViewModel>() },
                { "Tags manager", factory.Create<TagPickerViewModel>() },
                { "Parser", factory.Create<ParseViewModel>() },
                { "empty", null }
            };
            SelectedViewModel = ViewModels.FirstOrDefault().Value;
            SummaryViewModel = factory.Create<StockSummaryViewModel>();
        }
    }
}