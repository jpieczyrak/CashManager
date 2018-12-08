using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoTransactionType = CashManager.Data.DTO.TransactionType;
using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewModelFactory _factory;
        private readonly CategoryPickerViewModel _categoryPickerViewModel;
        private IEnumerable<Stock> _stocks;
        private Transaction _transaction;
        private bool _shouldCreateTransaction;
        private Tag[] _tags;

        public IEnumerable<TransactionType> TransactionTypes { get; set; }

        public Transaction Transaction
        {
            get => _transaction;
            set => Set(nameof(Transaction), ref _transaction, value);
        }

        public IEnumerable<Stock> ExternalStocks => _stocks.Where(x => !x.IsUserStock);

        public IEnumerable<Stock> UserStocks => _stocks.Where(x => x.IsUserStock);

        public RelayCommand<Position> ChooseCategoryCommand { get; set; }

        public RelayCommand SaveCommand { get; }

        public RelayCommand CancelCommand { get; }

        public RelayCommand AddNewPosition { get; }

        public TransactionViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher,
            ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _factory = factory;
            _categoryPickerViewModel = _factory.Create<CategoryPickerViewModel>();

            Update();

            ChooseCategoryCommand = new RelayCommand<Position>(position =>
            {
                var window = new CategoryPickerView(_categoryPickerViewModel, position.Category);
                window.Show();
                window.Closing += (sender, args) => { position.Category = _categoryPickerViewModel?.SelectedCategory; };
            });

            AddNewPosition = new RelayCommand(ExecuteAddPositionCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        private void ExecuteAddPositionCommand()
        {
            Transaction.Positions.Add(CreatePosition());
        }

        #region IUpdateable

        public void Update()
        {
            TransactionTypes = Mapper.Map<TransactionType[]>(_queryDispatcher
                                         .Execute<TransactionTypesQuery, DtoTransactionType[]>(new TransactionTypesQuery()))
                                     .OrderBy(x => x.InstanceCreationDate)
                                     .ToArray();
            _stocks = _queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()).Select(Mapper.Map<Stock>)
                                      .OrderBy(x => x.InstanceCreationDate)
                                      .ToArray();

            _tags = Mapper.Map<Tag[]>(_queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()))
                              .OrderBy(x => !x.IsSelected)
                              .ThenBy(x => x.Name)
                              .ToArray();

            if (_shouldCreateTransaction || Transaction == null) Transaction = CreateNewTransaction();

            foreach (var position in Transaction.Positions)
            {
                position.TagViewModel = _factory.Create<MultiComboBoxViewModel>();
                position.TagViewModel.SetInput(CopyOfTags(_tags), position.Tags);
            }
        }

        private BaseSelectable[] CopyOfTags(Tag[] tags) => Mapper.Map<Tag[]>(Mapper.Map<DtoTag[]>(tags));

        #endregion

        private Transaction CreateNewTransaction()
        {
            _shouldCreateTransaction = false;

            return new Transaction
            {
                Type = TransactionTypes.FirstOrDefault(x => x.IsDefault && x.Outcome),
                UserStock = UserStocks.FirstOrDefault(x => x.IsUserStock),
                ExternalStock = ExternalStocks.FirstOrDefault(),
                Positions = new TrulyObservableCollection<Position>(new[] { CreatePosition() } )
            };
        }

        private Position CreatePosition()
        {
            var position = new Position
            {
                Title = "new position",
                Category = _categoryPickerViewModel.Categories.FirstOrDefault(x => x.Parent == null),
                TagViewModel = _factory.Create<MultiComboBoxViewModel>()
            };
            position.TagViewModel.SetInput(CopyOfTags(_tags), position.Tags);

            return position;
        }

        private void ExecuteCancelCommand()
        {
            var transaction = _queryDispatcher
                              .Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery(x => x.Id == Transaction.Id))
                              .FirstOrDefault();
            if (transaction != null) Transaction = Mapper.Map<Transaction>(transaction);
            NavigateToTransactionListView();
        }

        private bool CanExecuteSaveCommand()
        {
            return !string.IsNullOrEmpty(Transaction.Title) && Transaction.Positions.Any() && Transaction.Type != null;
        }

        private void ExecuteSaveCommand()
        {
            foreach (var position in _transaction.Positions) position.Tags = position.TagViewModel.Results.OfType<Tag>().ToArray();
            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<DtoTransaction>(_transaction)));
            NavigateToTransactionListView();
            _shouldCreateTransaction = true;
        }

        private void NavigateToTransactionListView()
        {
            var applicationViewModel = _factory.Create<ApplicationViewModel>();
            var target = _factory.Create<TransactionListViewModel>();
            applicationViewModel.SetViewModelCommand.Execute(target);
        }
    }
}