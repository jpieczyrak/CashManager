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
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;
using CashManager_MVVM.Model.Setters;
using CashManager_MVVM.Properties;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Transaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.MassReplacer
{
    public class MassReplacerViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private TextSetter _titleSelector = new TextSetter(TextSetterType.Title);
        private TextSetter _noteSelector = new TextSetter(TextSetterType.Note);
        private TextSetter _positionTitleSelector = new TextSetter(TextSetterType.PositionTitle);
        private DateSetter _bookDateSetter = new DateSetter(DateSetterType.BookDate);
        private SinglePicker _userStocksSelector;
        private SinglePicker _externalStocksSelector;
        private SinglePicker _categoriesSelector;
        private SinglePicker _typesSelector;
        private MultiPicker _tagsSelector;

        public SearchViewModel SearchViewModel { get; private set; }

        public DateSetter BookDateSetter
        {
            get => _bookDateSetter;
            set => Set(nameof(BookDateSetter), ref _bookDateSetter, value);
        }

        public SinglePicker UserStocksSelector
        {
            get => _userStocksSelector;
            set => Set(nameof(UserStocksSelector), ref _userStocksSelector, value);
        }

        public SinglePicker ExternalStocksSelector
        {
            get => _externalStocksSelector;
            set => Set(nameof(ExternalStocksSelector), ref _externalStocksSelector, value);
        }

        public SinglePicker CategoriesSelector
        {
            get => _categoriesSelector;
            set => Set(nameof(CategoriesSelector), ref _categoriesSelector, value);
        }

        public SinglePicker TypesSelector
        {
            get => _typesSelector;
            set => Set(nameof(TypesSelector), ref _typesSelector, value);
        }

        public MultiPicker TagsSelector
        {
            get => _tagsSelector;
            set => Set(nameof(TagsSelector), ref _tagsSelector, value);
        }

        public TextSetter TitleSelector
        {
            get => _titleSelector;
            set => Set(nameof(TitleSelector), ref _titleSelector, value);
        }

        public TextSetter NoteSelector
        {
            get => _noteSelector;
            set => Set(nameof(NoteSelector), ref _noteSelector, value);
        }

        public TextSetter PositionTitleSelector
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
            return (_bookDateSetter.IsChecked
                     || (_userStocksSelector.IsChecked && _userStocksSelector.Selected != null)
                     || (_externalStocksSelector.IsChecked && _externalStocksSelector.Selected != null)
                     || (_titleSelector.IsChecked && !string.IsNullOrWhiteSpace(_titleSelector.Value))
                     || _noteSelector.IsChecked
                     || (_positionTitleSelector.IsChecked && !string.IsNullOrWhiteSpace(_positionTitleSelector.Value))
                     || (_categoriesSelector.IsChecked && _categoriesSelector.Selected != null)
                     || (_typesSelector.IsChecked && _typesSelector.Selected != null)
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

            if (_bookDateSetter.IsChecked)
                foreach (var transaction in transactions)
                    transaction.BookDate = _bookDateSetter.Value;

            if (_typesSelector.IsChecked && _typesSelector.Selected != null)
                foreach (var transaction in transactions)
                    transaction.Type = _typesSelector.Selected.Value as TransactionType;

            if (_userStocksSelector.IsChecked && _userStocksSelector.Selected != null)
                foreach (var transaction in transactions)
                    transaction.UserStock = _userStocksSelector.Selected.Value as Stock;
            if (_externalStocksSelector.IsChecked && _externalStocksSelector.Selected != null)
                foreach (var transaction in transactions)
                    transaction.ExternalStock = _externalStocksSelector.Selected.Value as Stock;

            var positions = SearchViewModel.IsTransactionsSearch
                                ? transactions.SelectMany(x => x.Positions).ToList()
                                : SearchViewModel.MatchingPositions;
            if (_positionTitleSelector.IsChecked && !string.IsNullOrWhiteSpace(_positionTitleSelector.Value))
                foreach (var position in positions)
                    position.Title = _positionTitleSelector.Value;
            if (_categoriesSelector.IsChecked && _categoriesSelector.Selected != null)
                foreach (var position in positions)
                    position.Category = _categoriesSelector.Selected.Value as Category;
            if (_tagsSelector.IsChecked)
                foreach (var position in positions)
                    position.Tags = _tagsSelector.Results.Select(x => x.Value as Tag).ToArray();

            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<Transaction[]>(transactions)));
        }

        public void Update()
        {
            var availableStocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksSelector = new SinglePicker(MultiPickerType.UserStock, availableStocks.Where(x => x.IsUserStock).Select(x => new Selectable(x)).ToArray());
            ExternalStocksSelector =
                new SinglePicker(MultiPickerType.ExternalStock,
                    Mapper.Map<Stock[]>(Mapper.Map<CashManager.Data.DTO.Stock[]>(availableStocks)).Select(x => new Selectable(x)).ToArray()); //we don't want to have same reference in 2 pickers

            var categories = Mapper.Map<Category[]>(_queryDispatcher.Execute<CategoryQuery, CashManager.Data.DTO.Category[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories);
            CategoriesSelector = new SinglePicker(MultiPickerType.Category, categories.Select(x => new Selectable(x)).ToArray());

            var types = Mapper.Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, CashManager.Data.DTO.TransactionType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesSelector = new SinglePicker(MultiPickerType.TransactionType, types.Select(x => new Selectable(x)).ToArray());

            var tags = Mapper.Map<Tag[]>(_queryDispatcher.Execute<TagQuery, CashManager.Data.DTO.Tag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsSelector = new MultiPicker(MultiPickerType.Tag, tags.Select(x => new Selectable(x)).ToArray());

            SearchViewModel.Update();
        }
    }
}