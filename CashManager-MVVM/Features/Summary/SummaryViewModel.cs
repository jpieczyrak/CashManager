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
        private const string AREA_TRACKER_FORMAT_STRING = "{2:MM.yyyy}\n{4:#,##0.00 zł}";
        private const string MONTH_DATE_FORMAT = "MM.yyyy";

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

            var values = GetBalances(x => new DateTime(x.BookDate.Year, x.BookDate.Month, 1));
            values = FillMissingMonthsWithZeroValue(values);

            if (values.Any())
            { 
                //todo: make switchable
                //SetColumns(values, BalanceModel);
                SetTwoColorArea(values, BalanceModel);
            }

            BalanceModel.InvalidatePlot(true);
            BalanceModel.ResetAllAxes();
        }

        private TransactionBalance[] GetBalances(Func<Transaction, DateTime> groupingSelector)
        {
            return _provider.AllTransactions.GroupBy(groupingSelector)
                            .Select(x => new TransactionBalance(x.Key, x.Sum(y => y.ValueAsProfit)))
                            .OrderBy(x => x.BookDate)
                            .ToArray();
        }

        private static TransactionBalance[] FillMissingMonthsWithZeroValue(TransactionBalance[] values)
        {
            var dict = values.ToDictionary(x => x.BookDate, x => x.Value);
            var actualDate = values.Min(x => x.BookDate);
            var maxDate = values.Max(x => x.BookDate);

            while (actualDate < maxDate)
            {
                if (!dict.ContainsKey(actualDate)) dict[actualDate] = 0;
                actualDate = actualDate.AddMonths(1);
            }

            values = dict.OrderBy(x => x.Key).Select(x => new TransactionBalance(x.Key, x.Value)).ToArray();
            return values;
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
                TrackerFormatString = AREA_TRACKER_FORMAT_STRING
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
                ItemsSource = values.Select(x => x.BookDate.ToString(MONTH_DATE_FORMAT)).ToArray(),
            });
        }
    }
}