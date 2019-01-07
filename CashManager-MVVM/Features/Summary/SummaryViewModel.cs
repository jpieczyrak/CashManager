using System;
using System.Linq;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Logic.Calculators;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Summary
{
    public class SummaryViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly TransactionsProvider _provider;
        private PlotModel _balanceModel;

        public PlotModel BalanceModel
        {
            get => _balanceModel;
            set => Set(ref _balanceModel, value);
        }

        public SummaryViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider provider)
        {
            _queryDispatcher = queryDispatcher;
            _provider = provider;
            BalanceModel = new PlotModel { IsLegendVisible = false };
            Update();
        }

        public void Update()
        {
            var stocks = _queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()).Where(x => x.IsUserStock);
            if (!stocks.Any()) return;

            BalanceModel.Series.Clear();
            BalanceModel.Axes.Clear();

            DateTime GroupingSelector(Transaction x) => new DateTime(x.BookDate.Year, x.BookDate.Month, 1);
            var values = _provider.AllTransactions.GroupBy(GroupingSelector)
                                  .Select(x => new TransactionBalance(x.Key, x.Sum(y => y.ValueAsProfit)))
                                  .OrderBy(x => x.BookDate)
                                  .ToArray();

            if (values.Any())
            {
                BalanceModel.Series.Add(new ColumnSeries
                {
                    ItemsSource = values,
                    ValueField = nameof(TransactionBalance.Value)
                });
                BalanceModel.Axes.Add(new CategoryAxis
                {
                    Position = AxisPosition.Bottom,
                    ItemsSource = values.Select(x => x.BookDate.ToString("MM.yyyy")).ToArray(),
                });
            }

            BalanceModel.InvalidatePlot(true);
            BalanceModel.ResetAllAxes();
        }
    }
}