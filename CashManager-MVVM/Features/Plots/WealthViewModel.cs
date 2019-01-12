using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CashManager.Infrastructure.Query;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Logic.Calculators;
using CashManager_MVVM.Model;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CashManager_MVVM.Features.Plots
{
    public class WealthViewModel : FilterableViewModel
    {
        private readonly TransactionBalanceCalculator _calculator;
        private PlotModel _wealth;

        public PlotModel Wealth
        {
            get => _wealth;
            set => Set(nameof(Wealth), ref _wealth, value);
        }

        public WealthViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider transactionsProvider,
            TransactionBalanceCalculator calculator) : base(queryDispatcher, transactionsProvider)
        {
            _calculator = calculator;
            Wealth = PlotHelper.CreatePlotModel();
            Wealth.Axes.Add(new DateTimeAxis());
        }

        public DataPoint[] GetWealthValues(IEnumerable<Transaction> transactions, Stock[] selectedStocks)
        {
            return _calculator.GetWealthValues(transactions, selectedStocks, BookDateFilter, x => x.BookDate);
        }

        #region Override

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var selectedStocks = UserStocksFilter.IsChecked
                                     ? UserStocksFilter.Results.OfType<Stock>().ToArray()
                                     : null;
            UpdateDateFilterRanges(selectedStocks, _transactionsProvider.AllTransactions);
            Wealth.Series.Clear();

            var values = GetWealthValues(_transactionsProvider.AllTransactions, selectedStocks);
            if (values.Any())
            {
                var series = new AreaSeries
                {
                    Title = "Wealth",
                    MarkerType = MarkerType.Cross
                };
                series.Points.AddRange(values);
                Wealth.Series.Add(series);
            }

            Wealth.InvalidatePlot(true);
            Wealth.ResetAllAxes();
        }

        private void UpdateDateFilterRanges(Stock[] selectedStocks, TrulyObservableCollection<Transaction> transactions)
        {
            var stocks = new HashSet<Stock>(selectedStocks);
            BookDateFilter.From = transactions.Where(x => stocks.Contains(x.UserStock)).Min(x => x.BookDate);
            BookDateFilter.To = transactions.Where(x => stocks.Contains(x.UserStock)).Max(x => x.BookDate);
        }

        #endregion
    }
}