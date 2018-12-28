using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Command.Transactions.Bills;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Infrastructure.Query.Transactions.Bills;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using GongSolutions.Wpf.DragDrop;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoTransactionType = CashManager.Data.DTO.TransactionType;
using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoStoredFileInfo = CashManager.Data.DTO.StoredFileInfo;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionViewModel : ViewModelBase, IUpdateable, IDropTarget
    {
        public TransactionsProvider TransactionsProvider { get; }
        //todo: on unload / hide etc - cancel changes to transaction

        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewModelFactory _factory;
        private readonly CategoryPickerViewModel _categoryPickerViewModel;
        private IEnumerable<Stock> _stocks;
        private Transaction _transaction;
        private bool _shouldCreateTransaction;
        private Tag[] _tags;

        public IEnumerable<TransactionType> TransactionTypes { get; set; }

        public ObservableCollection<string> NewBillsFilepaths { get; private set; }

        public ObservableCollection<BillImage> LoadedBills { get; private set; }

        public Transaction Transaction
        {
            get => _transaction;
            set
            {
                Set(nameof(Transaction), ref _transaction, value);
                _shouldCreateTransaction = false;
                if (_transaction != null)
                {
                    foreach (var position in _transaction.Positions)
                    {
                        position.CategoryPickerViewModel = new CategoryPickerViewModel(_queryDispatcher, position.Category);
                        //todo: check sender - only on selected category change
                        position.CategoryPickerViewModel.PropertyChanged +=
                            (sender, args) => position.Category = position.CategoryPickerViewModel.SelectedCategory;
                    }
                }
            }
        }

        public IEnumerable<Stock> ExternalStocks => _stocks.Where(x => !x.IsUserStock);

        public IEnumerable<Stock> UserStocks => _stocks.Where(x => x.IsUserStock);

        public RelayCommand<Position> RemovePositionCommand { get; set; }

        public RelayCommand SaveTransactionCommand { get; }

        public RelayCommand CancelTransactionCommand { get; }

        public RelayCommand AddNewPosition { get; }

        public bool ShouldGoBack { get; set; } = true;

        public TransactionViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher,
            ViewModelFactory factory, TransactionsProvider transactionsProvider)
        {
            TransactionsProvider = transactionsProvider;
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _factory = factory;
            _categoryPickerViewModel = _factory.Create<CategoryPickerViewModel>();

            NewBillsFilepaths = new ObservableCollection<string>();
            
            AddNewPosition = new RelayCommand(ExecuteAddPositionCommand);
            RemovePositionCommand = new RelayCommand<Position>(position => Transaction.Positions.Remove(position));

            SaveTransactionCommand = new RelayCommand(ExecuteSaveTransactionCommand, CanExecuteSaveTransactionCommand);
            CancelTransactionCommand = new RelayCommand(ExecuteCancelTransactionCommand);
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
            _shouldCreateTransaction = true;

            foreach (var position in Transaction.Positions)
            {
                position.TagViewModel = _factory.Create<MultiComboBoxViewModel>();
                position.TagViewModel.SetInput(CopyOfTags(_tags), position.Tags);
            }

            NewBillsFilepaths?.Clear();
            LoadedBills = new ObservableCollection<BillImage>(Transaction.StoredFiles.Select(CreateBillImage));
        }

        #endregion

        private void ExecuteAddPositionCommand()
        {
            Transaction.Positions.Add(CreatePosition());
        }

        private BaseSelectable[] CopyOfTags(Tag[] tags) => Mapper.Map<Tag[]>(Mapper.Map<DtoTag[]>(tags));

        private Transaction CreateNewTransaction()
        {
            return new Transaction
            {
                Type = TransactionTypes.FirstOrDefault(x => x.IsDefault && x.Outcome),
                UserStock = UserStocks.FirstOrDefault(x => x.IsUserStock),
                ExternalStock = ExternalStocks.FirstOrDefault(),
                Positions = new TrulyObservableCollection<Position>(new[] { CreatePosition() })
            };
        }

        private Position CreatePosition()
        {
            var category = _categoryPickerViewModel.Categories.FirstOrDefault(x => x.Parent == null);
            var position = new Position
            {
                Title = "new position",
                Category = category,
                TagViewModel = _factory.Create<MultiComboBoxViewModel>(),
                CategoryPickerViewModel = new CategoryPickerViewModel(_queryDispatcher, category)
            };
            //todo: check sender - only on selected category change
            position.CategoryPickerViewModel.PropertyChanged += 
                (sender, args) => position.Category = position.CategoryPickerViewModel.SelectedCategory;
            position.TagViewModel.SetInput(CopyOfTags(_tags), position.Tags);

            //todo: check if really needed
            position.Parent = Transaction;

            return position;
        }

        private void ExecuteCancelTransactionCommand()
        {
            var transaction = _queryDispatcher
                              .Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery(x => x.Id == Transaction.Id))
                              .FirstOrDefault();
            if (transaction != null) _transaction = Mapper.Map<Transaction>(transaction);
            NavigateBack();
        }

        private bool CanExecuteSaveTransactionCommand()
        {
            return !string.IsNullOrEmpty(Transaction.Title) && Transaction.Positions.Any() && Transaction.Type != null;
        }

        private void ExecuteSaveTransactionCommand()
        {
            foreach (var position in _transaction.Positions)
                position.Tags = position.TagViewModel.Results.OfType<Tag>().ToArray();

            var bills = NewBillsFilepaths.Select(x => new StoredFileInfo(x, Transaction.Id)).ToArray();
            _commandDispatcher.Execute(new UpsertBillsCommand(Mapper.Map<DtoStoredFileInfo[]>(bills)));
            NewBillsFilepaths.Clear();

            foreach (var bill in bills) if (!Transaction.StoredFiles.Contains(bill)) Transaction.StoredFiles.Add(bill);
            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<DtoTransaction>(_transaction)));

            //todo: make only 1 refresh
            TransactionsProvider.AllTransactions.Remove(Transaction);
            TransactionsProvider.AllTransactions.Add(Transaction);

            if (ShouldGoBack) NavigateBack();
        }

        private void NavigateBack()
        {
            _factory.Create<ApplicationViewModel>().GoBack();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = GetProperFilepaths(dropInfo).Any()
                                   ? DragDropEffects.Copy
                                   : DragDropEffects.None;
        }

        private string[] GetProperFilepaths(IDropInfo dropInfo)
        {
            string[] allowedExtensions = { ".jpg", ".png", ".bmp" };
            return ((DataObject) dropInfo.Data).GetFileDropList()
                                               .OfType<string>()
                                               .Where(x => !string.IsNullOrWhiteSpace(x))
                                               .Where(x => allowedExtensions.Contains(Path.GetExtension(x).ToLower()))
                                               .ToArray();
        }

        public void Drop(IDropInfo dropInfo)
        {
            var files = GetProperFilepaths(dropInfo);
            foreach (string file in files)
            {
                if (!NewBillsFilepaths.Contains(file)) NewBillsFilepaths.Add(file);
                var billImage = new BillImage(file, Path.GetFileNameWithoutExtension(file), File.ReadAllBytes(file));
                if (LoadedBills.Contains(billImage)) LoadedBills.Remove(billImage);
                LoadedBills.Add(billImage);
            }
        }
        
        private BillImage CreateBillImage(StoredFileInfo fileInfo)
        {
            var image = _queryDispatcher.Execute<BillQuery, byte[]>(new BillQuery(fileInfo.DbAlias));
            return new BillImage(fileInfo.SourceName, fileInfo.DisplayName, image);
        }
    }
}