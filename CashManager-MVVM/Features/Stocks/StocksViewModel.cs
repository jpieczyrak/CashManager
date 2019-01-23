using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.Logic.Creators;
using CashManager_MVVM.Messages.Models;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Stocks
{
    public class StocksViewModel : ViewModelBase, IUpdateable, IClosable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ICorrectionsCreator _correctionsCreator;

        public TrulyObservableCollection<Stock> Stocks { get; set; }

        public RelayCommand AddStockCommand { get; set; }

        public RelayCommand<Stock> RemoveCommand { get; set; }

        public StocksViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ICorrectionsCreator correctionsCreator)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;

            _correctionsCreator = correctionsCreator;
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

            var stock = Stocks.FirstOrDefault(x => x.Balance.Equals(balance));
            _correctionsCreator.CreateCorrection(stock, stock.Balance.Value - stock.Balance.PreviousValue);
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
            Stocks.CollectionChanged -= StocksOnCollectionChanged;
            if (Stocks != null) foreach (var stock in Stocks) stock.Balance.PropertyChanged -= BalanceOnPropertyChanged;
        }
    }
}