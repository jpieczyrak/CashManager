using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Features.TransactionTypes;
using CashManager_MVVM.Messages;
using CashManager_MVVM.Model;
using CashManager_MVVM.Properties;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Stocks
{
    public class StocksViewModel : ViewModelBase, IUpdateable, IClosable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly TransactionViewModel _transactionCreator;
        private TransactionTypesViewModel _typesProvider;

        public TrulyObservableCollection<Stock> Stocks { get; set; }

        public RelayCommand AddStockCommand { get; set; }

        public RelayCommand<Stock> RemoveCommand { get; set; }

        public StocksViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _transactionCreator = factory.Create<TransactionViewModel>();
            _typesProvider = factory.Create<TransactionTypesViewModel>();

            Update();

            AddStockCommand = new RelayCommand(() =>
            {
                var stock = new Stock();
                stock.Balance.PropertyChanged += BalanceOnPropertyChanged;
                Stocks.Add(stock);
            });
            RemoveCommand = new RelayCommand<Stock>(x =>
            {
                MessengerInstance.Send(new DeleteStockMessage(x));
                Stocks.Remove(x);

                _commandDispatcher.Execute(new DeleteStockCommand(Mapper.Map<DtoStock>(x)));
            },
            stock => Stocks.Count(x => x.IsUserStock) > 1);
            //todo: think what should happen on stock delete...
        }

        public void Update()
        {
            var stocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()))
                               .OrderBy(x => x.InstanceCreationDate)
                               .ToArray();
            Stocks = new TrulyObservableCollection<Stock>(stocks);
            foreach (var stock in Stocks) stock.Balance.PropertyChanged += BalanceOnPropertyChanged;
            Stocks.CollectionChanged += StocksOnCollectionChanged;
        }

        private void BalanceOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var balance = sender as Model.Balance;
            if (args.PropertyName != nameof(Model.Balance.Value) || balance == null) return;

            decimal diff = balance.Value - balance.PreviousValue;
            if (diff == 0m) return;

            var incomeTypes = _typesProvider.TransactionTypes
                                                 .Where(x => x.Income && !x.IsTransfer)
                                                 .OrderByDescending(x => x.IsDefault);
            var outcomeTypes = _typesProvider.TransactionTypes
                                            .Where(x => x.Outcome && !x.IsTransfer)
                                            .OrderByDescending(x => x.IsDefault);
            var transaction = new Transaction
            {
                Title = Strings.Correction,
                Note = Strings.ManualStockUpdate,
                BookDate = DateTime.Today,
                Type = diff > 0
                           ? incomeTypes.FirstOrDefault()
                           : outcomeTypes.FirstOrDefault(),
                UserStock = Stocks.FirstOrDefault(x => x.Balance.Equals(balance))
            };
            decimal abs = Math.Abs(diff);
            var position = new Position
            {
                BookDate = DateTime.Today,
                Value = new PaymentValue(abs, abs, 0m),
                Parent = transaction
            };
            transaction.Positions.Add(position);
            _transactionCreator.Transaction = transaction;
            _transactionCreator.SaveTransactionCommand.Execute(null);
        }

        private void StocksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                var updatedStocks = e.NewItems.OfType<Stock>().ToArray();
                MessengerInstance.Send(new UpdateStockMessage(updatedStocks));
                _commandDispatcher.Execute(new UpsertStocksCommand(Mapper.Map<DtoStock[]>(updatedStocks)));
            }
            else if (e.OldItems != null)
            {
                var deleted = e.OldItems.OfType<Stock>().ToArray();
                MessengerInstance.Send(new DeleteStockMessage(deleted));
            }
        }

        public void Close()
        {
            if (Stocks != null) foreach (var stock in Stocks) stock.Balance.PropertyChanged -= BalanceOnPropertyChanged;
        }
    }
}