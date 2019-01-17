using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Logic.Calculators;
using CashManager_MVVM.Model;
using CashManager_MVVM.Properties;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Plots
{
    public class WealthViewModel : FilterableViewModel
    {
        private const string AREA_TRACKER_DAY_FORMAT_STRING = "{2:dd.MM.yyyy}\n{4:#,##0.00 zł}";
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
            //todo: get it from some kind of stocks provider (instead of db)
            var ids = new HashSet<Guid>(UserStocksFilter.Results.Select(x => x.Id));
            var dtos = _queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery(x => ids.Contains(x.Id)));
            var selectedStocks = UserStocksFilter.IsChecked
                                     ? Mapper.Map<Stock[]>(dtos)
                                     : null;
            Wealth.Series.Clear();

            var values = GetWealthValues(_transactionsProvider.AllTransactions, selectedStocks);
            if (values.Any(x => x.Y > 0))
            {
                var series = new AreaSeries
                {
                    Title = Strings.Wealth,
                    MarkerType = MarkerType.Cross,
                    TrackerFormatString = AREA_TRACKER_DAY_FORMAT_STRING
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