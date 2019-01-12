﻿using System;
using System.Collections.Generic;
using System.Reflection;

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
        private string _title;
        private ViewModelBase _selectedViewModel;
        private readonly SummaryViewModel _summaryViewModel;
        private readonly StocksViewModel _stocksViewModel;
        private readonly CategoryManagerViewModel _categoryManagerViewModel;
        private readonly TransactionTypesViewModel _transactionTypesViewModel;
        private readonly TagManagerViewModel _tagManagerViewModel;
        private readonly ParserViewModel _parserViewModel;

        public ViewModelBase SelectedViewModel
        {
            get => _selectedViewModel;
            private set
            {
                PreviousSelectedViewModel = _selectedViewModel;
                Set(ref _selectedViewModel, value);
                if (_selectedViewModel is IUpdateable model) model.Update();
            }
        }

        public string Title
        {
            get => _title;
            private set => Set(ref _title, value);
        }

        private ViewModelBase PreviousSelectedViewModel { get; set; }

        public StockSummaryViewModel SummaryViewModel { get; }

        public Dictionary<string, ViewModelBase> ViewModels { get; private set; }

        public RelayCommand<ViewModelBase> SetViewModelCommand { get; private set; }

        public RelayCommand<ViewModel> SelectViewModelCommand { get; }

        public ApplicationViewModel(ViewModelFactory factory)
        {
            Title += $"Cash Manager {Assembly.GetExecutingAssembly().GetName().Version}";
            SetViewModelCommand = new RelayCommand<ViewModelBase>(view => SelectedViewModel = view);

            _summaryViewModel = factory.Create<SummaryViewModel>();
            _stocksViewModel = factory.Create<StocksViewModel>();
            _categoryManagerViewModel = factory.Create<CategoryManagerViewModel>();
            _transactionTypesViewModel = factory.Create<TransactionTypesViewModel>();
            _tagManagerViewModel = factory.Create<TagManagerViewModel>();
            _parserViewModel = factory.Create<ParserViewModel>();
            ViewModels = new Dictionary<string, ViewModelBase>
            {
                { "Transactions search", factory.Create<SearchViewModel>() },
                { "Mass replacer", factory.Create<MassReplacerViewModel>() },
                { "Add transaction", factory.Create<TransactionViewModel>() },
                { "Wealth plot", factory.Create<WealthViewModel>() },
                { "Categories plot", factory.Create<CategoriesPlotViewModel>() },
                { "Custom balances", factory.Create<CustomBalanceViewModel>() }
            };
            PreviousSelectedViewModel = SelectedViewModel = _summaryViewModel;

            SummaryViewModel = factory.Create<StockSummaryViewModel>();

            SelectViewModelCommand = new RelayCommand<ViewModel>(ExecuteSelectViewModelCommand);
        }

        private void ExecuteSelectViewModelCommand(ViewModel viewModel)
        {
            switch (viewModel)
            {
                case ViewModel.Summary:
                    SelectedViewModel = _summaryViewModel;
                    break;
                case ViewModel.StockManager:
                    SelectedViewModel = _stocksViewModel;
                    break;
                case ViewModel.CategoryManager:
                    SelectedViewModel = _categoryManagerViewModel;
                    break;
                case ViewModel.TypesManager:
                    SelectedViewModel = _transactionTypesViewModel;
                    break;
                case ViewModel.TagsManager:
                    SelectedViewModel = _tagManagerViewModel;
                    break;
                case ViewModel.About:
                    //new AboutWindow().Show();
                    break;
                case ViewModel.Import:
                    SelectedViewModel = _parserViewModel;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewModel), viewModel, null);
            }
        }

        public void GoBack() { SelectedViewModel = PreviousSelectedViewModel; }
    }
}