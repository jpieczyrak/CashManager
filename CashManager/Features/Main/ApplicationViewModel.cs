using System;
using System.Linq;
using System.Reflection;

using AutoMapper;

using CashManager.Data.DTO;
using CashManager.Features.Balance;
using CashManager.Features.Categories;
using CashManager.Features.Main.Settings;
using CashManager.Features.MassReplacer;
using CashManager.Features.Parsers;
using CashManager.Features.Parsers.Custom;
using CashManager.Features.Plots;
using CashManager.Features.Search;
using CashManager.Features.Stocks;
using CashManager.Features.Summary;
using CashManager.Features.Tags;
using CashManager.Features.Transactions;
using CashManager.Features.TransactionTypes;
using CashManager.Logic.Parsers;
using CashManager.Logic.Parsers.Custom.Predefined;
using CashManager.Logic.Wrappers;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager.Features.Main
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
        private readonly Lazy<ParserViewModelBase> _parserViewModel;
        private readonly Lazy<RegexParserViewModel> _regexParser;
        private readonly Lazy<CsvParserViewModel> _csvParser;
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
            Title = $"Cash Manager {Assembly.GetExecutingAssembly().GetName().Version} - BETA";

            _summaryViewModel = new Lazy<SummaryViewModel>(() => _factory.Create<SummaryViewModel>());
            _stocksViewModel = new Lazy<StocksViewModel>(() => _factory.Create<StocksViewModel>());
            _categoryManagerViewModel = new Lazy<CategoryManagerViewModel>(() => _factory.Create<CategoryManagerViewModel>());
            _transactionTypesViewModel = new Lazy<TransactionTypesViewModel>(() => _factory.Create<TransactionTypesViewModel>());
            _tagManagerViewModel = new Lazy<TagManagerViewModel>(() => _factory.Create<TagManagerViewModel>());
            _parserViewModel = new Lazy<ParserViewModelBase>(() => _factory.Create<ParserViewModelBase>());
            _regexParser = new Lazy<RegexParserViewModel>(() => _factory.Create<RegexParserViewModel>());
            _csvParser = new Lazy<CsvParserViewModel>(() => _factory.Create<CsvParserViewModel>());
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
                case ViewModel.ImportGetinWeb:
                    _parserViewModel.Value.Parser = new GetinBankParser();
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                case ViewModel.ImportIdeaWeb:
                    _parserViewModel.Value.Parser = new IdeaBankParser();
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                case ViewModel.ImportIngWeb:
                    _parserViewModel.Value.Parser = new IngBankParser();
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                case ViewModel.ImportIntelligoWeb:
                    _parserViewModel.Value.Parser = new IntelligoBankParser();
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                case ViewModel.ImportMillenniumWeb:
                    _parserViewModel.Value.Parser = new MillenniumBankParser();
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                case ViewModel.ImportIngCsv:
                {
                    var factory = new CustomCsvParserFactory(Mapper.Map<Stock[]>(_parserViewModel.Value.UserStocks.Concat(_parserViewModel.Value.ExternalStocks)));
                    _parserViewModel.Value.Parser = factory.Create(PredefinedCsvParsers.Ing);
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                }
                case ViewModel.ImportMillenniumCsv:
                {
                    var factory = new CustomCsvParserFactory(Mapper.Map<Stock[]>(_parserViewModel.Value.UserStocks.Concat(_parserViewModel.Value.ExternalStocks)));
                    _parserViewModel.Value.Parser = factory.Create(PredefinedCsvParsers.Millennium);
                    SelectedViewModel = _parserViewModel.Value;
                    break;
                }
                case ViewModel.ImportCustomRegex:
                    SelectedViewModel = _regexParser.Value;
                    break;
                case ViewModel.ImportCustomCsv:
                    SelectedViewModel = _csvParser.Value;
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