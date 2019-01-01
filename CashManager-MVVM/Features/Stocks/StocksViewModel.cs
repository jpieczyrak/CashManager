using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.Messages;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Stocks
{
    public class StocksViewModel : ViewModelBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public TrulyObservableCollection<Stock> Stocks { get; }

        public RelayCommand AddStockCommand { get; set; }

        public RelayCommand<Stock> RemoveCommand { get; set; }

        public StocksViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;

            var stocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()))
                               .OrderBy(x => x.InstanceCreationDate)
                               .ToArray();
            Stocks = new TrulyObservableCollection<Stock>(stocks);
            Stocks.CollectionChanged += StocksOnCollectionChanged;

            AddStockCommand = new RelayCommand(() =>
            {
                var stock = new Stock();
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
    }
}