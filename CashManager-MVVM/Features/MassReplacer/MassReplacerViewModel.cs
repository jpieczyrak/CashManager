using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Transaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.MassReplacer
{
    public class MassReplacerViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private TextSelector _titleSelector = new TextSelector(TextSelectorType.Title);
        private TextSelector _noteSelector = new TextSelector(TextSelectorType.Note);
        private TextSelector _positionTitleSelector = new TextSelector(TextSelectorType.PositionTitle);
        private DateSelector _bookDateSelector = new DateSelector("Book date");
        private MultiPicker _userStocksSelector;
        private MultiPicker _externalStocksSelector;
        private MultiPicker _categoriesSelector;
        private MultiPicker _typesSelector;
        private MultiPicker _tagsSelector;

        public SearchViewModel SearchViewModel { get; private set; }

        public DateSelector BookDateSelector
        {
            get => _bookDateSelector;
            set => Set(nameof(BookDateSelector), ref _bookDateSelector, value);
        }

        public MultiPicker UserStocksSelector
        {
            get => _userStocksSelector;
            set => Set(nameof(UserStocksSelector), ref _userStocksSelector, value);
        }

        public MultiPicker ExternalStocksSelector
        {
            get => _externalStocksSelector;
            set => Set(nameof(ExternalStocksSelector), ref _externalStocksSelector, value);
        }

        public MultiPicker CategoriesSelector
        {
            get => _categoriesSelector;
            set => Set(nameof(CategoriesSelector), ref _categoriesSelector, value);
        }

        public MultiPicker TypesSelector
        {
            get => _typesSelector;
            set => Set(nameof(TypesSelector), ref _typesSelector, value);
        }

        public MultiPicker TagsSelector
        {
            get => _tagsSelector;
            set => Set(nameof(TagsSelector), ref _tagsSelector, value);
        }

        public TextSelector TitleSelector
        {
            get => _titleSelector;
            set => Set(nameof(TitleSelector), ref _titleSelector, value);
        }

        public TextSelector NoteSelector
        {
            get => _noteSelector;
            set => Set(nameof(NoteSelector), ref _noteSelector, value);
        }

        public TextSelector PositionTitleSelector
        {
            get => _positionTitleSelector;
            set => Set(nameof(PositionTitleSelector), ref _positionTitleSelector, value);
        }

        public RelayCommand PerformCommand { get; }

        public MassReplacerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            SearchViewModel = factory.Create<SearchViewModel>();
            PerformCommand = new RelayCommand(ExecutePerformCommand, CanExecutePerformCommand);
        }

        private bool CanExecutePerformCommand()
        {
            return (_bookDateSelector.IsChecked
                     || (_userStocksSelector.IsChecked && _userStocksSelector.Results.Any())
                     || (_externalStocksSelector.IsChecked && _externalStocksSelector.Results.Any())
                     || (_titleSelector.IsChecked && !string.IsNullOrWhiteSpace(_titleSelector.Value))
                     || _noteSelector.IsChecked
                     || (_positionTitleSelector.IsChecked && !string.IsNullOrWhiteSpace(_positionTitleSelector.Value))
                     || (_categoriesSelector.IsChecked && _categoriesSelector.Results.Any())
                     || (_typesSelector.IsChecked && _typesSelector.Results.Any())
                     || _tagsSelector.IsChecked)
                   && SearchViewModel.MatchingTransactions.Any();
        }

        private void ExecutePerformCommand()
        {
            var transactions = SearchViewModel.MatchingTransactions;

            if (_titleSelector.IsChecked && !string.IsNullOrWhiteSpace(_titleSelector.Value))
                foreach (var transaction in transactions)
                    transaction.Title = _titleSelector.Value;
            if (_noteSelector.IsChecked)
                foreach (var transaction in transactions)
                    transaction.Note = _noteSelector.Value;

            if (_bookDateSelector.IsChecked)
                foreach (var transaction in transactions)
                    transaction.BookDate = _bookDateSelector.Value;

            if (_typesSelector.IsChecked && _typesSelector.Results.Any())
                foreach (var transaction in transactions)
                    transaction.Type = _typesSelector.Results.OfType<TransactionType>().FirstOrDefault();

            if (_userStocksSelector.IsChecked && _userStocksSelector.Results.Any())
                foreach (var transaction in transactions)
                    transaction.UserStock = _userStocksSelector.Results.OfType<Stock>().FirstOrDefault();
            if (_externalStocksSelector.IsChecked && _externalStocksSelector.Results.Any())
                foreach (var transaction in transactions)
                    transaction.ExternalStock = _externalStocksSelector.Results.OfType<Stock>().FirstOrDefault();

            var positions = SearchViewModel.IsTransactionsSearch
                                ? transactions.SelectMany(x => x.Positions).ToList()
                                : SearchViewModel.MatchingPositions;
            if (_positionTitleSelector.IsChecked && !string.IsNullOrWhiteSpace(_positionTitleSelector.Value))
                foreach (var position in positions)
                    position.Title = _positionTitleSelector.Value;
            if (_categoriesSelector.IsChecked && _categoriesSelector.Results.Any())
                foreach (var position in positions)
                    position.Category = _categoriesSelector.Results.OfType<Category>().FirstOrDefault();
            if (_tagsSelector.IsChecked)
                foreach (var position in positions)
                    position.Tags = _tagsSelector.Results.OfType<Tag>().ToArray();

            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<Transaction[]>(transactions)));
        }

        public void Update()
        {
            var availableStocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksSelector = new MultiPicker(MultiPickerType.UserStock, availableStocks.Where(x => x.IsUserStock).ToArray());
            ExternalStocksSelector =
                new MultiPicker(MultiPickerType.ExternalStock,
                    Mapper.Map<Stock[]>(Mapper.Map<CashManager.Data.DTO.Stock[]>(availableStocks))); //we don't want to have same reference in 2 pickers

            var categories = Mapper.Map<Category[]>(_queryDispatcher.Execute<CategoryQuery, CashManager.Data.DTO.Category[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories);
            CategoriesSelector = new MultiPicker(MultiPickerType.Category, categories);

            var types = Mapper.Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, CashManager.Data.DTO.TransactionType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesSelector = new MultiPicker(MultiPickerType.TransactionType, types);

            var tags = Mapper.Map<Tag[]>(_queryDispatcher.Execute<TagQuery, CashManager.Data.DTO.Tag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsSelector = new MultiPicker(MultiPickerType.Tag, tags);

            SearchViewModel.Update();
        }
    }
}