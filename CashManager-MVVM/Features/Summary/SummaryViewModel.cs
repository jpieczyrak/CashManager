using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CashManager.Infrastructure.Query;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Features.Plots;
using CashManager_MVVM.Logic.Calculators;
using CashManager_MVVM.Model;

using log4net;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace CashManager_MVVM.Features.Summary
{
    public class SummaryViewModel : FilterableViewModel
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(SummaryViewModel)));

        private const string AREA_TRACKER_MONTH_FORMAT_STRING = "{2:MM.yyyy}\n{4:#,##0.00 zł}";
        private const string AREA_TRACKER_YEAR_FORMAT_STRING = "{2:yyyy}\n{4:#,##0.00 zł}";
        private const string MONTH_DATE_FORMAT = "MM.yyyy";

        public PlotModel BalanceModel { get; }
        public PlotModel FlowsModel { get; }
        public PlotModel YearBalanceModel { get; }

        public TransactionsSummary[] Balances { get; private set; }

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

        public SummaryViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider provider) 
            : base(queryDispatcher, provider)
        {
            BalanceModel = PlotHelper.CreatePlotModel();
            BalanceModel.IsLegendVisible = false;
            FlowsModel = PlotHelper.CreatePlotModel();
            FlowsModel.IsLegendVisible = false;
            YearBalanceModel = PlotHelper.CreatePlotModel();
            YearBalanceModel.IsLegendVisible = false;

            BalanceModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, IntervalType = DateTimeIntervalType.Months });
            FlowsModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, IntervalType = DateTimeIntervalType.Months });
            YearBalanceModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, IsAxisVisible = false });
            YearBalanceModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, IntervalType = DateTimeIntervalType.Years });

            Update();
        }

        public sealed override void Update()
        {
            _logger.Value.Debug("Update");
            base.Update();
            _logger.Value.Debug("Updated");
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            BalanceModel.Series.Clear();
            FlowsModel.Series.Clear();
            YearBalanceModel.Series.Clear();
            
            if (MatchingTransactions.Any())
            {
                var minDate = MatchingTransactions.Min(x => x.BookDate);
                var maxDate = MatchingTransactions.Max(x => x.BookDate);

                var balances = GetTransactions(TimeGroupingType.Month, minDate, maxDate, TransactionTypeSelection.Balance);
                SetTwoColorArea(balances, BalanceModel, OxyColors.Green, TimeGroupingType.Month);
                //SetColumns(values, BalanceModel); //todo: make switchable

                var incomes = GetTransactions(TimeGroupingType.Month, minDate, maxDate, TransactionTypeSelection.Income);
                var outcomes = GetTransactions(TimeGroupingType.Month, minDate, maxDate, TransactionTypeSelection.Outcome);

                SetTwoColorArea(incomes, FlowsModel, OxyColors.Green, TimeGroupingType.Month);
                SetTwoColorArea(outcomes, FlowsModel, OxyColors.Red, TimeGroupingType.Month);

                SetTwoColorArea(GetTransactions(TimeGroupingType.Year, minDate, maxDate, TransactionTypeSelection.Balance), YearBalanceModel, OxyColors.Green, TimeGroupingType.Year);

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

            BalanceModel.InvalidatePlot(true); BalanceModel.ResetAllAxes();
            FlowsModel.InvalidatePlot(true); FlowsModel.ResetAllAxes();
            YearBalanceModel.InvalidatePlot(true); YearBalanceModel.ResetAllAxes();
        }

        private TransactionBalance[] GetTransactions(TimeGroupingType grouping, DateTime minDate, DateTime maxDate,
            TransactionTypeSelection selection)
        {
            var groupingSelector = GetGroupingSelector(grouping);

            Func<Transaction, bool> valuesPicker = null;

            switch (selection)
            {
                case TransactionTypeSelection.Income:
                    valuesPicker = x => x.ValueAsProfit > 0;
                    break;
                case TransactionTypeSelection.Outcome:
                    valuesPicker = x => x.ValueAsProfit < 0;
                    break;
                case TransactionTypeSelection.Balance:
                    valuesPicker = x => true;
                    break;
            }

            return FillMissingEntriesWithZeroValue(
                MatchingTransactions
                    .Where(valuesPicker)
                    .GroupBy(x => groupingSelector(x.BookDate))
                    .Select(x => new TransactionBalance(x.Key, x.Sum(y => y.ValueAsProfit)))
                    .OrderBy(x => x.BookDate), minDate, maxDate, grouping);
        }

        private static Func<DateTime, DateTime> GetGroupingSelector(TimeGroupingType grouping)
        {
            switch (grouping)
            {
                case TimeGroupingType.Month:
                    return x => new DateTime(x.Year, x.Month, 1);
                case TimeGroupingType.Year:
                    return x => new DateTime(x.Year, 1, 1);
            }

            return null;
        }

        private static TransactionBalance[] FillMissingEntriesWithZeroValue(IEnumerable<TransactionBalance> values, DateTime minDate,
            DateTime maxDate, TimeGroupingType grouping)
        {
            var selector = GetGroupingSelector(grouping);
            var dict = values.ToDictionary(x => selector(x.BookDate), x => x.Value);
            int startMonth = grouping == TimeGroupingType.Month ? minDate.Month : 1;
            var actualDate = new DateTime(minDate.Year, startMonth, 1);

            while (actualDate < maxDate)
            {
                if (!dict.ContainsKey(actualDate)) dict[actualDate] = 0;
                switch (grouping)
                {
                    case TimeGroupingType.Month:
                        actualDate = actualDate.AddMonths(1);
                        break;
                    case TimeGroupingType.Year:
                        actualDate = actualDate.AddYears(1);
                        break;
                }
            }

            values = dict.OrderBy(x => x.Key).Select(x => new TransactionBalance(x.Key, x.Value)).ToArray();
            return values.ToArray();
        }

        private void SetTwoColorArea(TransactionBalance[] values, PlotModel model, OxyColor color, TimeGroupingType groupingType)
        {
            //lets make it rectangle area by adding same value and the end of the month
            var rectValues = CreateDuplicateTransactionBalancesAtEndOfGroupingPeriod(values, groupingType);

            model.Series.Add(new TwoColorAreaSeries
            {
                ItemsSource = rectValues,
                Limit = 0,
                Color = color,
                Color2 = OxyColors.Red,
                Mapping = x => new DataPoint(DateTimeAxis.ToDouble(((TransactionBalance) x).BookDate),
                    (double) ((TransactionBalance) x).Value),
                TrackerFormatString = groupingType == TimeGroupingType.Month
                                          ? AREA_TRACKER_MONTH_FORMAT_STRING
                                          : AREA_TRACKER_YEAR_FORMAT_STRING
            });
        }

        private static TransactionBalance[] CreateDuplicateTransactionBalancesAtEndOfGroupingPeriod(TransactionBalance[] values,
            TimeGroupingType grouping)
        {
            Func<TransactionBalance, TransactionBalance> selector = null;
            switch (grouping)
            {
                case TimeGroupingType.Month:
                    selector = x => new TransactionBalance(x.BookDate.AddMonths(1).AddSeconds(-1), x.Value);
                    break;
                case TimeGroupingType.Year:
                    selector = x => new TransactionBalance(x.BookDate.AddYears(1).AddSeconds(-1), x.Value);
                    break;
            }

            if (selector == null) return values;
            var rectValues = values.Concat(values.Select(selector))
                                   .OrderBy(x => x.BookDate)
                                   .ToArray();
            return rectValues;
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