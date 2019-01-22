﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Stocks;
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
using CashManager_MVVM.Messages.Models;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Properties;
using CashManager_MVVM.UserCommunication;
using CashManager_MVVM.Utils;

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
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewModelFactory _factory;
        private readonly IMessagesService _messagesService;
        private readonly CategoryPickerViewModel _categoryPickerViewModel;
        private IEnumerable<Stock> _stocks;
        private Transaction _transaction;
        private bool _shouldCreateTransaction;

        private bool _isInEditMode;

        public bool IsInEditMode
        {
            get => _isInEditMode;
            private set => Set(ref _isInEditMode, value);
        }

        public Dictionary<TransactionEditModes, TransactionEditMode> Modes { get; }

        private Tag[] _tags;

        private bool _updateStock;
        private decimal _startTransactionValue;
        private Stock _startUserStock;

        public TransactionsProvider TransactionsProvider { get; }

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

                        //todo: check sender - only on selected category change (no on all changes)
                        position.CategoryPickerViewModel.PropertyChanged +=
                            (sender, args) => position.Category = position.CategoryPickerViewModel.SelectedCategory;
                    }

                    _startTransactionValue = _transaction.ValueAsProfit;
                    _startUserStock = _transaction.UserStock;
                    IsInEditMode = true;
                }
            }
        }

        public IEnumerable<Stock> ExternalStocks => _stocks.Where(x => !x.IsUserStock);

        public IEnumerable<Stock> UserStocks => _stocks.Where(x => x.IsUserStock);

        public RelayCommand<Position> RemovePositionCommand { get; set; }

        public RelayCommand SaveTransactionCommand { get; }

        public RelayCommand CancelTransactionCommand { get; }

        public RelayCommand AddNewPosition { get; }
        public RelayCommand ClearCommand { get; }

        public bool ShouldGoBack { private get; set; } = true;

        public bool UpdateStock
        {
            get => _updateStock;
            set => Set(ref _updateStock, value);
        }

        public TransactionViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher,
            ViewModelFactory factory, TransactionsProvider transactionsProvider, IMessagesService messagesService)
        {
            UpdateStock = true;
            Modes = new Dictionary<TransactionEditModes, TransactionEditMode>
            {
                [TransactionEditModes.NoChange] = new TransactionEditMode
                {
                    Name = "No change",
                    Type = TransactionEditModes.NoChange,
                    IsSelected = true
                },
                [TransactionEditModes.AddCorrection] = new TransactionEditMode
                {
                    Name = "Add correction transaction",
                    Type = TransactionEditModes.AddCorrection
                },
                [TransactionEditModes.ChangeStockBalance] = new TransactionEditMode
                {
                    Name = "Just change stock balance",
                    Type = TransactionEditModes.ChangeStockBalance
                }
            };
            TransactionsProvider = transactionsProvider;
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _factory = factory;
            _messagesService = messagesService;
            _categoryPickerViewModel = _factory.Create<CategoryPickerViewModel>();

            NewBillsFilepaths = new ObservableCollection<string>();

            AddNewPosition = new RelayCommand(ExecuteAddPositionCommand);
            RemovePositionCommand = new RelayCommand<Position>(ExecuteRemovePositionCommand);

            SaveTransactionCommand = new RelayCommand(ExecuteSaveTransactionCommand, CanExecuteSaveTransactionCommand);
            CancelTransactionCommand = new RelayCommand(ExecuteCancelTransactionCommand);

            ClearCommand = new RelayCommand(ExecuteClearCommand);
        }

        private void ExecuteRemovePositionCommand(Position position)
        {
            if (Settings.Default.QuestionForPositionDelete)
                if (!_messagesService.ShowQuestionMessage(Strings.Question, string.Format(Strings.QuestionDoYouWantToRemovePositionFormat, position.Title)))
                    return;
            Transaction.Positions.Remove(position);
        }

        private void ExecuteClearCommand()
        {
            Transaction = CreateNewTransaction();
            IsInEditMode = false;
        }

        #region IDropTarget

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = GetProperFilepaths(dropInfo).Any()
                                   ? DragDropEffects.Copy
                                   : DragDropEffects.None;
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

        #endregion

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
                          .OrderBy(x => x.Name)
                          .ToArray();

            if (_shouldCreateTransaction || Transaction == null)
            {
                Transaction = CreateNewTransaction();
                IsInEditMode = false;
            }
            _shouldCreateTransaction = true;

            foreach (var position in Transaction.Positions)
            {
                position.TagViewModel = _factory.Create<MultiComboBoxViewModel>();
                position.TagViewModel.SetInput(CopyOfTags(_tags), position.Tags.Select(x => new Selectable(x)).ToArray());
            }

            NewBillsFilepaths?.Clear();
            LoadedBills = new ObservableCollection<BillImage>(Transaction.StoredFiles.Select(CreateBillImage));
        }

        #endregion

        public void ExecuteAddPositionCommand() => Transaction.Positions.Add(CreatePosition(Transaction));

        private Selectable[] CopyOfTags(Tag[] tags) => tags.Select(x => new Selectable(x)).ToArray();

        private Transaction CreateNewTransaction()
        {
            var transaction = new Transaction
            {
                Type = TransactionTypes.FirstOrDefault(x => x.IsDefault && x.Outcome),
                UserStock = UserStocks.FirstOrDefault(x => x.IsUserStock),
                ExternalStock = ExternalStocks.FirstOrDefault()
            };

            transaction.Positions = new TrulyObservableCollection<Position>(new[] { CreatePosition(transaction) });
            return transaction;
        }

        private Position CreatePosition(Transaction parent)
        {
            var category = _categoryPickerViewModel.DefaultCategory;
            var position = new Position
            {
                Title = Strings.NewPosition,
                Category = category,
                TagViewModel = _factory.Create<MultiComboBoxViewModel>(),
                CategoryPickerViewModel = new CategoryPickerViewModel(_queryDispatcher, category)
            };
            //todo: check sender - only on selected category change
            position.CategoryPickerViewModel.PropertyChanged +=
                (sender, args) => position.Category = position.CategoryPickerViewModel.SelectedCategory;
            position.TagViewModel.SetInput(CopyOfTags(_tags), position.Tags.Select(x => new Selectable(x)).ToArray());

            position.Parent = parent;

            return position;
        }

        private void ExecuteCancelTransactionCommand()
        {
            var transaction = _queryDispatcher
                              .Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery(x => x.Id == Transaction.Id))
                              .FirstOrDefault();
            if (transaction == null)
            {
                _shouldCreateTransaction = false;
            }
            else
            {
                _transaction = Mapper.Map<Transaction>(transaction);
                var found = TransactionsProvider.AllTransactions.FirstOrDefault(x => x.Id == _transaction.Id);
                if (found != null)
                {
                    TransactionsProvider.AllTransactions.Remove(found);
                    TransactionsProvider.AllTransactions.Add(_transaction);
                }
            }

            NavigateBack();
        }

        private bool CanExecuteSaveTransactionCommand() => Transaction.IsValid;

        private void ExecuteSaveTransactionCommand()
        {
            foreach (var position in _transaction.Positions.Where(x => x.TagViewModel != null))
                position.Tags = Mapper.Map<Tag[]>(position.TagViewModel.Results);

            var bills = NewBillsFilepaths.Select(x => new StoredFileInfo(x, Transaction.Id)).ToArray();
            _commandDispatcher.Execute(new UpsertBillsCommand(Mapper.Map<DtoStoredFileInfo[]>(bills)));
            NewBillsFilepaths.Clear();

            foreach (var bill in bills)
                if (!Transaction.StoredFiles.Contains(bill))
                    Transaction.StoredFiles.Add(bill);
            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<DtoTransaction>(_transaction)));

            //todo: make only 1 refresh
            TransactionsProvider.AllTransactions.Remove(Transaction);
            TransactionsProvider.AllTransactions.Add(Transaction);

            HandleStocksValueUpdate();

            SoundPlayerHelper.PlaySound(SoundPlayerHelper.Sound.AddTransaction);

            if (ShouldGoBack) NavigateBack();
        }

        private void HandleStocksValueUpdate()
        {
            if (UpdateStock)
            {
                var updatedStocks = new[] { Transaction.UserStock }.ToList();
                if (_startUserStock == null || _startUserStock.Equals(Transaction.UserStock))
                {
                    Transaction.UserStock.Balance.Value += (Transaction.ValueWithSign - _startTransactionValue);
                }
                else
                {
                    _startUserStock.Balance.Value -= _startTransactionValue;
                    Transaction.UserStock.Balance.Value += Transaction.ValueWithSign;
                    updatedStocks.Add(_startUserStock);
                }

                _commandDispatcher.Execute(new UpsertStocksCommand(Mapper.Map<DtoStock[]>(updatedStocks)));
                MessengerInstance.Send(new UpdateStockMessage(updatedStocks.ToArray()));
            }
        }

        private void NavigateBack()
        {
            _factory.Create<ApplicationViewModel>().GoBack();
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

        private BillImage CreateBillImage(StoredFileInfo fileInfo)
        {
            var image = _queryDispatcher.Execute<BillQuery, byte[]>(new BillQuery(fileInfo.DbAlias));
            return new BillImage(fileInfo.SourceName, fileInfo.DisplayName, image);
        }
    }
}