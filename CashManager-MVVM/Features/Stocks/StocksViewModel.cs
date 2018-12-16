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

            var stocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery()))
                               .OrderBy(x => x.InstanceCreationDate)
                               .ToArray();
            Stocks = new TrulyObservableCollection<Stock>(stocks);
            Stocks.CollectionChanged += StocksOnCollectionChanged;

            AddStockCommand = new RelayCommand(() => { Stocks.Add(new Stock()); });
            RemoveCommand = new RelayCommand<Stock>(x =>
            {
                x.IsUserStock = false; //hack to make update message remove the stock for summary todo: fix
                Stocks.Remove(x);

                _commandDispatcher.Execute(new DeleteStockCommand(Mapper.Map<CashManager.Data.DTO.Stock>(x)));
            });
        }

        private void StocksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var stocks = Stocks.Select(Mapper.Map<CashManager.Data.DTO.Stock>).ToArray();
            _commandDispatcher.Execute(new UpsertStocksCommand(stocks));

            MessengerInstance.Send(new StockUpdateMessage(Stocks.ToArray()));
        }
    }
}