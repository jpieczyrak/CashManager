﻿using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Tags;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

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
        private readonly CategoryViewModel _categoryViewModel;
        private IEnumerable<Stock> _stocks;
        private Transaction _transaction;
        private bool _shouldCreateTransaction;

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
            _categoryViewModel = _factory.Create<CategoryViewModel>();

            Update();

            ChooseCategoryCommand = new RelayCommand<Position>(position =>
            {
                var window = new CategoryPickerView(_categoryViewModel, position.Category);
                window.Show();
                window.Closing += (sender, args) => { position.Category = _categoryViewModel?.SelectedCategory; };
            });

            AddNewPosition = new RelayCommand(() => Transaction.Positions.Add(new Position { Title = "new" }));
            SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        #region IUpdateable

        public void Update()
        {
            TransactionTypes = Mapper.Map<TransactionType[]>(_queryDispatcher
                                         .Execute<TransactionTypesQuery, DtoTransactionType[]>(new TransactionTypesQuery()))
                                     .OrderBy(x => x.InstanceCreationDate)
                                     .ToArray(); ;
            _stocks = _queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()).Select(Mapper.Map<Stock>)
                                      .OrderBy(x => x.InstanceCreationDate)
                                      .ToArray();
            
            if (_shouldCreateTransaction || Transaction == null) Transaction = CreateNewTransaction();

            foreach (var position in Transaction.Positions)
            {
                position.TagViewModel = _factory.Create<TagPickerViewModel>();
                position.TagViewModel.SelectTags(position.Tags);
            }
        }

        #endregion

        private Transaction CreateNewTransaction()
        {
            _shouldCreateTransaction = false;
            return new Transaction
            {
                Type = TransactionTypes.FirstOrDefault(x => x.IsDefault && x.Outcome),
                UserStock = UserStocks.FirstOrDefault(x => x.IsUserStock),
                ExternalStock = ExternalStocks.FirstOrDefault(),
                Positions = new TrulyObservableCollection<Position>(new[]
                {
                    new Position
                    {
                        Title = "empty",
                        Category = _categoryViewModel.Categories.FirstOrDefault(x => x.Parent == null),
                        TagViewModel = _factory.Create<TagPickerViewModel>()
                    }
                })
            };
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
            foreach (var position in _transaction.Positions) position.Tags = position.TagViewModel.SelectedTags;
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