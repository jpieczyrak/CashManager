using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Logic.Calculators;
using CashManager.Model;
using CashManager.Properties;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager.Features.Plots
{
    public class WealthViewModel : FilterableViewModel
    {
        private readonly string _areaTrackerDayFormatString = $"{{2:dd.MM.yyyy}}\n{{4:{Strings.ValueFormat}}}";

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

        public DataPoint[] GetWealthValues(IEnumerable<Transaction> transactions, Stock[] selectedStocks, bool showTransfers)
        {
            return _calculator.GetWealthValues(transactions, selectedStocks, BookDateFilter, x => x.BookDate, showTransfers);
        }

        #region Override

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            //todo: get it from some kind of stocks provider (instead of db)
            var ids = new HashSet<Guid>(UserStocksFilter.Results.Select(x => x.Id));
            var dtos = _queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery(x => ids.Contains(x.Id)));
            var selectedStocks = UserStocksFilter.IsChecked
                                     ? Mapper.Map<Stock[]>(dtos)
                                     : null;
            Wealth.Series.Clear();

            var values = GetWealthValues(_transactionsProvider.AllTransactions, selectedStocks, ShowTransfers);
            if (values.Any(x => x.Y > 0))
            {
                var series = new AreaSeries
                {
                    Title = Strings.Wealth,
                    MarkerType = MarkerType.Cross,
                    TrackerFormatString = _areaTrackerDayFormatString
                };
                series.Points.AddRange(values);
                Wealth.Series.Add(series);
            }

            Wealth.InvalidatePlot(true);
            Wealth.ResetAllAxes();
        }

        #endregion
    }
}