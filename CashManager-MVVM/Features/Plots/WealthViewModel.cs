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
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Plots
{
    public class WealthViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly TransactionsProvider _transactionsProvider;
        private readonly TransactionBalanceCalculator _calculator;
        private DateFrame _bookDateFilter;
        private MultiPicker _userStocksFilter;
        private PlotModel _wealth;

        public DateFrame BookDateFilter
        {
            get => _bookDateFilter;
            set => Set(nameof(BookDateFilter), ref _bookDateFilter, value);
        }

        public MultiPicker UserStocksFilter
        {
            get => _userStocksFilter;
            set => Set(nameof(UserStocksFilter), ref _userStocksFilter, value);
        }

        public PlotModel Wealth
        {
            get => _wealth;
            set => Set(nameof(Wealth), ref _wealth, value);
        }

        public WealthViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider transactionsProvider, TransactionBalanceCalculator calculator)
        {
            _queryDispatcher = queryDispatcher;
            _transactionsProvider = transactionsProvider;
            _calculator = calculator;
            _bookDateFilter = new DateFrame(DateFrameType.BookDate);
            Wealth = PlotHelper.CreatePlotModel();
            Wealth.Axes.Add(new DateTimeAxis());
        }

        public void Update()
        {
            var dtos = _queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery());
            var stocks = Mapper.Map<Stock[]>(dtos)
                               .Where(x => x.IsUserStock)
                               .OrderBy(x => x.Name)
                               .ToArray();

            if (UserStocksFilter != null) UserStocksFilter.PropertyChanged -= OnPropertyChanged;
            UserStocksFilter = new MultiPicker(MultiPickerType.UserStock, stocks);
            foreach (var result in UserStocksFilter.ComboBox.InternalDisplayableSearchResults) result.IsSelected = true;
            UserStocksFilter.IsChecked = true;
            UserStocksFilter.PropertyChanged += OnPropertyChanged;
            
            BookDateFilter.PropertyChanged -= OnPropertyChanged;
            BookDateFilter.From = _transactionsProvider.AllTransactions.Any()
                                      ? _transactionsProvider.AllTransactions.Min(x => x.BookDate).AddDays(-1)
                                      : DateTime.MinValue;
            BookDateFilter.To = _transactionsProvider.AllTransactions.Any()
                                    ? _transactionsProvider.AllTransactions.Max(x => x.BookDate)
                                    : DateTime.MaxValue;

            BookDateFilter.IsChecked = true;
            BookDateFilter.PropertyChanged += OnPropertyChanged;

            OnPropertyChanged(this, null);
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var selectedStocks = UserStocksFilter.IsChecked
                                     ? UserStocksFilter.Results.OfType<Stock>().ToArray()
                                     : null;
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

        public DataPoint[] GetWealthValues(IEnumerable<Transaction> transactions, Stock[] selectedStocks)
        {
            return _calculator.GetWealthValues(transactions, selectedStocks, BookDateFilter, x => x.BookDate);
        }
    }
}