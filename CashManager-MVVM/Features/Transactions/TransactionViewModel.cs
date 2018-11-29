using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Data;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Transactions;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewModelFactory _factory;
        private readonly IEnumerable<Stock> _stocks;
        private Transaction _transaction;

        public IEnumerable<TransactionType> TransactionTypes { get; }

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

        public TransactionViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher,
            ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _factory = factory;

            TransactionTypes = null; //todo: transaction types query

            var dtos = _queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery());
            _stocks = dtos.Select(Mapper.Map<Stock>);

            ChooseCategoryCommand = new RelayCommand<Position>(position =>
            {
                var viewModel = _factory.Create<CategoryViewModel>();
                var window = new CategoryPickerView(viewModel, position.Category);
                window.Show();
                window.Closing += (sender, args) => { position.Category = viewModel?.SelectedCategory; };
            });

            SaveCommand = new RelayCommand(() =>
            {
                _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<CashManager.Data.DTO.Transaction>(_transaction)));
                NavigateToTransactionListView();
            });

            CancelCommand = new RelayCommand(() =>
            {
                var transaction = _queryDispatcher
                                  .Execute<TransactionQuery, CashManager.Data.DTO.Transaction[]>(new TransactionQuery())
                                  .FirstOrDefault();
                Transaction = Mapper.Map<Transaction>(transaction);
                NavigateToTransactionListView();
            });
        }

        public void NavigateToTransactionListView()
        {
            var applicationViewModel = _factory.Create<ApplicationViewModel>();
            var target = _factory.Create<TransactionListViewModel>();
            applicationViewModel.SetViewModelCommand.Execute(target);
        }
    }
}