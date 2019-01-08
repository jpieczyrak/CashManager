using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Features.Plots;
using CashManager_MVVM.Logic.Calculators;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

using log4net;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Summary
{
    public class SummaryViewModel : FilterableViewModel
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(SummaryViewModel)));

        private const string AREA_TRACKER_FORMAT_STRING = "{2:MM.yyyy}\n{4:#,##0.00 zł}";
        private const string MONTH_DATE_FORMAT = "MM.yyyy";

        public PlotModel BalanceModel { get; }
        public PlotModel FlowsModel { get; }

        public TransactionsSummary[] Balances { get; private set; }

        public SummaryViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider provider) 
            : base(queryDispatcher, provider)
        {
            BalanceModel = PlotHelper.CreatePlotModel();
            BalanceModel.IsLegendVisible = false;
            FlowsModel = PlotHelper.CreatePlotModel();
            FlowsModel.IsLegendVisible = false;

            BalanceModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalType = DateTimeIntervalType.Months
            });
            FlowsModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalType = DateTimeIntervalType.Months
            });

            Update();
        }

        public override void Update()
        {
            _logger.Value.Debug("Update");
            base.Update();
            _logger.Value.Debug("Updated");
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            BalanceModel.Series.Clear();
            FlowsModel.Series.Clear();

            Func<Transaction, DateTime> groupingSelector = x => new DateTime(x.BookDate.Year, x.BookDate.Month, 1);
            var values = GetBalances(groupingSelector);
            values = FillMissingMonthsWithZeroValue(values, values.Min(x => x.BookDate), values.Max(x => x.BookDate));

            if (values.Any())
            {
                //todo: make switchable
                //SetColumns(values, BalanceModel);
                SetTwoColorArea(values, BalanceModel, OxyColors.Green);

                var minDate = MatchingTransactions.Min(x => x.BookDate);
                var maxDate = MatchingTransactions.Max(x => x.BookDate);
                var incomes = FillMissingMonthsWithZeroValue(
                    MatchingTransactions
                             .Where(x => x.ValueAsProfit > 0)
                             .GroupBy(groupingSelector)
                             .Select(x => new TransactionBalance(x.Key, x.Sum(y => y.ValueAsProfit)))
                             .OrderBy(x => x.BookDate), minDate, maxDate);
                var outcomes = FillMissingMonthsWithZeroValue(
                    MatchingTransactions
                             .Where(x => x.ValueAsProfit < 0)
                             .GroupBy(groupingSelector)
                             .Select(x => new TransactionBalance(x.Key, x.Sum(y => y.ValueAsProfit)))
                             .OrderBy(x => x.BookDate), minDate, maxDate);

                SetTwoColorArea(incomes, FlowsModel, OxyColors.Green);
                SetTwoColorArea(outcomes, FlowsModel, OxyColors.Red);

                Balances = incomes
                           .OrderByDescending(x => x.BookDate)
                           .Zip(outcomes.OrderByDescending(x => x.BookDate), (income, outcome) =>
                               new TransactionsSummary
                               {
                                   GrossIncome = income.Value,
                                   GrossOutcome = -outcome.Value,
                                   Name = income.BookDate.ToString("yyyy.MM")
                               })
                           .ToArray();
                RaisePropertyChanged(nameof(Balances));
            }

            BalanceModel.InvalidatePlot(true);
            BalanceModel.ResetAllAxes();

            FlowsModel.InvalidatePlot(true);
            FlowsModel.ResetAllAxes();
        }

        private TransactionBalance[] GetBalances(Func<Transaction, DateTime> groupingSelector)
        {
            return MatchingTransactions
                   .GroupBy(groupingSelector)
                   .Select(x => new TransactionBalance(x.Key, x.Sum(y => y.ValueAsProfit)))
                   .OrderBy(x => x.BookDate)
                   .ToArray();
        }

        private IEnumerable<Transaction> MatchingTransactions
        {
            get
            {
                var stockHashSet = new HashSet<Stock>(UserStocksFilter.Results.OfType<Stock>());
                return _transactionsProvider.AllTransactions
                                            .Where(x => !UserStocksFilter.IsChecked || stockHashSet.Contains(x.UserStock))
                                            .Where(x => !BookDateFilter.IsChecked
                                                        || x.BookDate >= BookDateFilter.From 
                                                        && x.BookDate <= BookDateFilter.To);
            }
        }

        private static TransactionBalance[] FillMissingMonthsWithZeroValue(IEnumerable<TransactionBalance> values, DateTime minDate, DateTime maxDate)
        {
            var dict = values.ToDictionary(x => new DateTime(x.BookDate.Year, x.BookDate.Month, 1), x => x.Value);
            var actualDate = new DateTime(minDate.Year, minDate.Month, 1);

            while (actualDate < maxDate)
            {
                if (!dict.ContainsKey(actualDate)) dict[actualDate] = 0;
                actualDate = actualDate.AddMonths(1);
            }

            values = dict.OrderBy(x => x.Key).Select(x => new TransactionBalance(x.Key, x.Value)).ToArray();
            return values.ToArray();
        }

        private void SetTwoColorArea(TransactionBalance[] values, PlotModel model, OxyColor color)
        {
            //lets make it rectangle area by adding same value and the end of the month
            var rectValues = values.Concat(values.Select(x => new TransactionBalance(x.BookDate.AddMonths(1).AddSeconds(-1), x.Value)))
                                   .OrderBy(x => x.BookDate)
                                   .ToArray();

            model.Series.Add(new TwoColorAreaSeries
            {
                ItemsSource = rectValues,
                Limit = 0,
                Color = color,
                Color2 = OxyColors.Red,
                Mapping = x => new DataPoint(DateTimeAxis.ToDouble(((TransactionBalance) x).BookDate),
                    (double) ((TransactionBalance) x).Value),
                TrackerFormatString = AREA_TRACKER_FORMAT_STRING
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