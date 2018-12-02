using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Stocks
{
    public class StockSummaryViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public Stock[] Stocks { get; }

        public double Total => Stocks?.Sum(x => x.Balance.Value) ?? 0;

        public StockSummaryViewModel(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            Stocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery()))
                           .Where(x => x.IsUserStock)
                           .OrderBy(x => x.InstanceCreationDate)
                           .ToArray();
        }
    }
}