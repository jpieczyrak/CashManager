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
                //SetColumns(values, BalanceModel); //todo: make switchable
                SetTwoColorArea(values, BalanceModel);
            }

            BalanceModel.InvalidatePlot(true);
            BalanceModel.ResetAllAxes();
        }

        private void SetTwoColorArea(TransactionBalance[] values, PlotModel model)
        {
            //lets make it rectangle area by adding same value and the end of the month
            var rectValues = values.Concat(values.Select(x => new TransactionBalance(x.BookDate.AddMonths(1).AddSeconds(-1), x.Value)))
                           .OrderBy(x => x.BookDate)
                           .ToArray();

            model.Series.Add(new TwoColorAreaSeries
            {
                ItemsSource = rectValues,
                Limit = 0,
                Color2 = OxyColors.Red,
                Mapping = x => new DataPoint(DateTimeAxis.ToDouble(((TransactionBalance) x).BookDate),
                    (double) ((TransactionBalance) x).Value),
                TrackerFormatString = "{2:MM.yyyy}\n{4:#,##0.00 zł}"
            });
            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalType = DateTimeIntervalType.Months
            });
        }

        private void SetColumns(TransactionBalance[] values, PlotModel model)
        {
            model.Series.Add(new ColumnSeries
            {
                ItemsSource = values,
                ValueField = nameof(TransactionBalance.Value)
            });
            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = values.Select(x => x.BookDate.ToString("MM.yyyy")).ToArray(),
            });
        }
    }
}