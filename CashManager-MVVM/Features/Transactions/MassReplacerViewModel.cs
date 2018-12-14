using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Transactions
{
    public class MassReplacerViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ViewModelFactory _factory;
        private TextSelector _titleSelector = new TextSelector("Title");
        private TextSelector _noteSelector = new TextSelector("Note");
        private DateSelector _bookDateSelector = new DateSelector("Book date");
        private MultiPicker _userStocksSelector;
        private MultiPicker _externalStocksSelector;
        private MultiPicker _categoriesSelector;
        private MultiPicker _typesSelector;
        private MultiPicker _tagsSelector;

        public TransactionSearchViewModel TransactionsSearchViewModel { get; private set; }

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

        public RelayCommand PerformCommand { get; }

        public MassReplacerViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _factory = factory;
            TransactionsSearchViewModel = _factory.Create<TransactionSearchViewModel>();
            PerformCommand = new RelayCommand(ExecutePerformCommand, CanExecutePerformCommand);

            Update();
        }

        private bool CanExecutePerformCommand()
        {
            return (_bookDateSelector.IsChecked 
                   || _categoriesSelector.IsChecked 
                   || _externalStocksSelector.IsChecked
                   || _noteSelector.IsChecked
                   || _tagsSelector.IsChecked
                   || _titleSelector.IsChecked
                   || _typesSelector.IsChecked
                   || _userStocksSelector.IsChecked)
                && TransactionsSearchViewModel.Transactions.Any();
        }

        private void ExecutePerformCommand()
        {
            var transactions = TransactionsSearchViewModel.Transactions;

            if (_titleSelector.IsChecked && !string.IsNullOrWhiteSpace(_titleSelector.Value))
                foreach (var transaction in transactions)
                    transaction.Title = _titleSelector.Value;
            if (_noteSelector.IsChecked)
                foreach (var transaction in transactions)
                    transaction.Note = _noteSelector.Value;
            if (_bookDateSelector.IsChecked)
                foreach (var transaction in transactions)
                    transaction.BookDate = _bookDateSelector.Value;
            if (_typesSelector.IsChecked)
                foreach (var transaction in transactions)
                    transaction.Type = _typesSelector.Results.OfType<TransactionType>().FirstOrDefault();

            if (_userStocksSelector.IsChecked && _userStocksSelector.Results.Any())
                foreach (var transaction in transactions)
                    transaction.UserStock = _userStocksSelector.Results.OfType<Stock>().FirstOrDefault();
            if (_externalStocksSelector.IsChecked && _externalStocksSelector.Results.Any())
                foreach (var transaction in transactions)
                    transaction.UserStock = _externalStocksSelector.Results.OfType<Stock>().FirstOrDefault();


            if (_categoriesSelector.IsChecked && _categoriesSelector.Results.Any())
            {
                foreach (var position in transactions.SelectMany(x => x.Positions))
                {
                    position.Category = _categoriesSelector.Results.OfType<Category>().FirstOrDefault();
                }
            }
            if (_tagsSelector.IsChecked)
            {
                foreach (var position in transactions.SelectMany(x => x.Positions))
                {
                    position.Tags = _tagsSelector.Results.OfType<Tag>().ToArray();
                }
            }

            //todo: save
            //todo: notify???
            //var all = TransactionsSearchViewModel.TransactionsListViewModel.Transactions.ToArray();
            //TransactionsSearchViewModel.TransactionsListViewModel.Transactions.Clear();
            //var updatedSource = TransactionsSearchViewModel.Transactions
            //TransactionsSearchViewModel.TransactionsListViewModel.Transactions.Add();
        }

        public void Update()
        {
            var availableStocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksSelector = new MultiPicker("User stock", availableStocks.Where(x => x.IsUserStock).ToArray());
            ExternalStocksSelector =
                new MultiPicker("External stock",
                    Mapper.Map<Stock[]>(Mapper.Map<CashManager.Data.DTO.Stock[]>(availableStocks))); //we don't want to have same reference in 2 pickers

            var categories = Mapper.Map<Category[]>(_queryDispatcher.Execute<CategoryQuery, CashManager.Data.DTO.Category[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories).ToArray();
            CategoriesSelector = new MultiPicker("Categories", categories);

            var types = Mapper.Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, CashManager.Data.DTO.TransactionType[]>(new TransactionTypesQuery())
                                                                     .OrderBy(x => x.Name));
            TypesSelector = new MultiPicker("Types", types);

            var tags = Mapper.Map<Tag[]>(_queryDispatcher.Execute<TagQuery, CashManager.Data.DTO.Tag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsSelector = new MultiPicker("Tags", tags);
        }
    }
}