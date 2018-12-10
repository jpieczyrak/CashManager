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
                PreviousSelectedViewModel = _selectedViewModel;
                Set(ref _selectedViewModel, value, nameof(SelectedViewModel));
                if (_selectedViewModel is IUpdateable model) model.Update();
            }
        }

        public ViewModelBase PreviousSelectedViewModel { get; private set; }

        public StockSummaryViewModel SummaryViewModel { get; }
        public Dictionary<string, ViewModelBase> ViewModels { get; private set; }

        public RelayCommand<ViewModelBase> SetViewModelCommand { get; private set; }

        public ApplicationViewModel(ViewModelFactory factory)
        {
            SetViewModelCommand = new RelayCommand<ViewModelBase>(view => SelectedViewModel = view);
            ViewModels = new Dictionary<string, ViewModelBase>
            {
                { "Transactions search", factory.Create<TransactionSearchViewModel>() },
                { "Transactions list", factory.Create<TransactionListViewModel>() },
                { "Add transaction", factory.Create<TransactionViewModel>() },
                { "Stocks manager", factory.Create<StocksViewModel>() },
                { "Category manager", factory.Create<CategoryManagerViewModel>() },
                { "Types manager", factory.Create<TransactionTypesViewModel>() },
                { "Tags manager", factory.Create<TagManagerViewModel>() },
                { "Parser", factory.Create<ParseViewModel>() }
            };
            PreviousSelectedViewModel = SelectedViewModel = ViewModels.FirstOrDefault().Value;
            SummaryViewModel = factory.Create<StockSummaryViewModel>();
        }

        public void GoBack()
        {
            SelectedViewModel = PreviousSelectedViewModel;
        }
    }
}