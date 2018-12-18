using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.Messages;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Stocks
{
    public class StockSummaryViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private Stock[] _stocks;

        public Stock[] Stocks
        {
            get => _stocks;
            private set => Set(nameof(Stocks), ref _stocks, value);
        }

        public decimal Total => Stocks?.Sum(x => x.UserBalance) ?? 0m;

        public StockSummaryViewModel(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            Stocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery()))
                           .Where(x => x.IsUserStock)
                           .OrderBy(x => x.InstanceCreationDate)
                           .ToArray();
            MessengerInstance.Register<StockUpdateMessage>(this, Update);
        }

        private void Update(StockUpdateMessage message)
        {
            var updated = message.UpdatedStocks.ToArray();
            Stocks = Stocks.Except(updated)
                           .Concat(updated)
                           .Where(x => x.IsUserStock)
                           .OrderBy(x => x.InstanceCreationDate)
                           .ToArray();
            RaisePropertyChanged(nameof(Total));
        }
    }
}