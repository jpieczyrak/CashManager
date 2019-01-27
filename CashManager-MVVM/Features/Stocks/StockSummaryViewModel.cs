using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager.WPF.Messages.Models;
using CashManager.WPF.Model;

using GalaSoft.MvvmLight;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager.WPF.Features.Stocks
{
    public class StockSummaryViewModel : ViewModelBase
    {
        private Stock[] _stocks;

        public Stock[] Stocks
        {
            get => _stocks;
            private set => Set(nameof(Stocks), ref _stocks, value);
        }

        public decimal Total => Stocks?.Sum(x => x.UserBalance) ?? 0m;

        public StockSummaryViewModel(IQueryDispatcher queryDispatcher)
        {
            var query = new StockQuery();
            var stocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(query));
            Stocks = FilterAndOrderStocks(stocks);

            MessengerInstance.Register<UpdateStockMessage>(this, Update);
            MessengerInstance.Register<DeleteStockMessage>(this, Delete);
        }

        private void Update(UpdateStockMessage message)
        {
            var updated = message.UpdatedStocks;
            Stocks = FilterAndOrderStocks(Stocks.Except(updated).Concat(updated));
            RaisePropertyChanged(nameof(Total));
        }

        private void Delete(DeleteStockMessage message)
        {
            Stocks = FilterAndOrderStocks(Stocks.Except(message.DeletedStocks));
            RaisePropertyChanged(nameof(Total));
        }

        private Stock[] FilterAndOrderStocks(IEnumerable<Stock> stocks)
        {
            return stocks
                   .Where(x => x.IsUserStock)
                   .OrderBy(x => x.InstanceCreationDate)
                   .ToArray();
        }
    }
}