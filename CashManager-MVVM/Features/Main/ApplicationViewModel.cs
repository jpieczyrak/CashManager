﻿using System;
using System.Linq;
using System.Reflection;

using CashManager.Logic.Wrappers;

using CashManager_MVVM.Features.Balance;
using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.MassReplacer;
using CashManager_MVVM.Features.Parsers;
using CashManager_MVVM.Features.Plots;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Features.Stocks;
using CashManager_MVVM.Features.Summary;
using CashManager_MVVM.Features.Tags;
using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Features.TransactionTypes;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Main
{
    public class ApplicationViewModel : ViewModelBase
    {
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable it have to be field to make commands work properly
        private readonly ViewModelFactory _factory;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
        private string _title;
        private AboutWindow _aboutWindow;
        private ViewModelBase _selectedViewModel;
        private readonly Lazy<SummaryViewModel> _summaryViewModel;
        private readonly Lazy<StocksViewModel> _stocksViewModel;
        private readonly Lazy<CategoryManagerViewModel> _categoryManagerViewModel;
        private readonly Lazy<TransactionTypesViewModel> _transactionTypesViewModel;
        private readonly Lazy<TagManagerViewModel> _tagManagerViewModel;
        private readonly Lazy<ParserViewModel> _parserViewModel;
        private readonly Lazy<SettingsViewModel> _settingsViewModel;
        private readonly Lazy<SearchViewModel> _searchViewModel;
        private readonly Lazy<MassReplacerViewModel> _massReplacerViewModel;
        private readonly Lazy<WealthViewModel> _wealthPlotViewModel;
        private readonly Lazy<CategoriesPlotViewModel> _categoriesPlotViewModel;
        private readonly Lazy<CustomBalanceViewModel> _customBalancesViewModel;

        public ViewModelBase SelectedViewModel
        {
            get => _selectedViewModel;
            private set
            {
                PreviousSelectedViewModel = _selectedViewModel;
                if (_selectedViewModel is IClosable closable)
                {
                    string name = closable.GetType().ToString().Split('.').LastOrDefault();
                    using (new MeasureTimeWrapper(() => closable.Close(), $"{name}.Close")) { }
                }
                Set(ref _selectedViewModel, value);
                if (_selectedViewModel is IUpdateable model)
                {
                    string name = model.GetType().ToString().Split('.').LastOrDefault();
                    using (new MeasureTimeWrapper(() => model.Update(), $"{name}.Update")) { }
                }
            }
        }

        public string Title
        {
            get => _title;
            private set => Set(ref _title, value);
        }

        private ViewModelBase PreviousSelectedViewModel { get; set; }

        public Lazy<TransactionViewModel> TransactionViewModel { get; }

        public RelayCommand<ViewModel> SelectViewModelCommand { get; }

        public NotificationViewModel NotificationViewModel { get; }

        public ApplicationViewModel(ViewModelFactory factory)
        {
            _factory = factory;
            Title = $"Cash Manager {Assembly.GetExecutingAssembly().GetName().Version}";

            _summaryViewModel = new Lazy<SummaryViewModel>(() => _factory.Create<SummaryViewModel>());
            _stocksViewModel = new Lazy<StocksViewModel>(() => _factory.Create<StocksViewModel>());
            _categoryManagerViewModel = new Lazy<CategoryManagerViewModel>(() => _factory.Create<CategoryManagerViewModel>());
            _transactionTypesViewModel = new Lazy<TransactionTypesViewModel>(() => _factory.Create<TransactionTypesViewModel>());
            _tagManagerViewModel = new Lazy<TagManagerViewModel>(() => _factory.Create<TagManagerViewModel>());
            _parserViewModel = new Lazy<ParserViewModel>(() => _factory.Create<ParserViewModel>());
            _settingsViewModel = new Lazy<SettingsViewModel>(_factory.Create<SettingsViewModel>);
            _searchViewModel = new Lazy<SearchViewModel>(_factory.Create<SearchViewModel>);
            _massReplacerViewModel = new Lazy<MassReplacerViewModel>(_factory.Create<MassReplacerViewModel>);
            TransactionViewModel = new Lazy<TransactionViewModel>(_factory.Create<TransactionViewModel>);
            _wealthPlotViewModel = new Lazy<WealthViewModel>(_factory.Create<WealthViewModel>);
            _categoriesPlotViewModel = new Lazy<CategoriesPlotViewModel>(_factory.Create<CategoriesPlotViewModel>);
            _customBalancesViewModel = new Lazy<CustomBalanceViewModel>(_factory.Create<CustomBalanceViewModel>);
            PreviousSelectedViewModel = SelectedViewModel = _summaryViewModel.Value;
            NotificationViewModel = _factory.Create<NotificationViewModel>();

            SelectViewModelCommand = new RelayCommand<ViewModel>(ExecuteSelectViewModelCommand);
        }

        private void ExecuteSelectViewModelCommand(ViewModel viewModel)
        {
            switch (viewModel)
            {
                case ViewModel.Summary:
                    SelectedViewModel = _summaryViewModel.Value;
                    break;
                case ViewModel.StockManager:
                    SelectedViewModel = _stocksViewModel.Value;
                    break;
                case ViewModel.CategoryManager:
                    SelectedViewModel = _categoryManagerViewModel.Value;
                    break;
                case ViewModel.TypesManager:
                    SelectedViewModel = _transactionTypesViewModel.Value;
                    break;
                case ViewModel.TagsManager:
                    SelectedViewModel = _tagManagerViewModel.Value;
                    break;
                case ViewModel.About:
                    if (_aboutWindow == null || _aboutWindow.IsVisible == false) _aboutWindow = new AboutWindow();
                    _aboutWindow.Show();
                    break;
                case ViewModel.Settings:
                    SelectedViewModel = _settingsViewModel.Value;
                    break;
                case ViewModel.Import:
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                case ViewModel.Search:
                    SelectedViewModel = _searchViewModel.Value;
                    break;
                case ViewModel.MassReplacer:
                    SelectedViewModel = _massReplacerViewModel.Value;
                    break;
                case ViewModel.Transaction:
                    SelectedViewModel = TransactionViewModel.Value;
                    break;
                case ViewModel.WealthPlot:
                    SelectedViewModel = _wealthPlotViewModel.Value;
                    break;
                case ViewModel.CategoriesPlot:
                    SelectedViewModel = _categoriesPlotViewModel.Value;
                    break;
                case ViewModel.CustomBalances:
                    SelectedViewModel = _customBalancesViewModel.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewModel), viewModel, null);
            }
        }

        public void GoBack() { SelectedViewModel = PreviousSelectedViewModel; }
    }
}