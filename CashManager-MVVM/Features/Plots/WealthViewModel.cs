﻿using System;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Transactions;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;
using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.Plots
{
    public class WealthViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private Transaction[] _allTransactions;
        private DateFrame _bookDateFilter = new DateFrame("Book date");
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

        public WealthViewModel(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            Wealth = new PlotModel();
            Wealth.Axes.Add(new DateTimeAxis());
            Update();
        }

        public void Update()
        {
            _allTransactions = Mapper.Map<Transaction[]>(_queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()));
            
            var stocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()))
                               .Where(x => x.IsUserStock)
                               .OrderBy(x => x.Name)
                               .ToArray();
            UserStocksFilter = new MultiPicker("User stock", stocks);
            foreach (var result in UserStocksFilter.ComboBox.InternalDisplayableSearchResults) result.IsSelected = true;
            UserStocksFilter.IsChecked = true;
            UserStocksFilter.PropertyChanged += OnPropertyChanged;

            BookDateFilter.From = _allTransactions.Min(x => x.BookDate);
            BookDateFilter.To = _allTransactions.Max(x => x.BookDate);
            BookDateFilter.IsChecked = true;
            BookDateFilter.PropertyChanged += OnPropertyChanged;

            OnPropertyChanged(this, null);
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Wealth.Series.Clear();
            if (_allTransactions == null || !_allTransactions.Any()) return;

            var selectedStocks = UserStocksFilter.IsChecked
                                     ? UserStocksFilter.Results.OfType<Stock>().ToArray()
                                     : null;

            if (selectedStocks != null && selectedStocks.Any())
            {
                DateTime stockDate = selectedStocks.Max(x => x.LastEditDate);
                decimal actualValue = selectedStocks.Sum(x => x.Balance.Value);

                var values = _allTransactions
                             .Where(x => selectedStocks.Contains(x.UserStock))
                             .OrderByDescending(x => x.BookDate)
                             .GroupBy(x => x.BookDate)
                             .Select(x => new { BookDate = x.Key, Value = x.Sum(y => y.ValueAsProfit) })
                             .Where(x => !BookDateFilter.IsChecked || x.BookDate >= BookDateFilter.From && x.BookDate <= BookDateFilter.To)
                             .Select(x =>
                             {
                                 actualValue -= x.Value;
                                 double value = (double) actualValue;
                                 return new DataPoint(DateTimeAxis.ToDouble(x.BookDate), value);
                             })
                             .OrderBy(x => x.X)
                             .Concat(!BookDateFilter.IsChecked || stockDate.Date <= BookDateFilter.To
                                         ? new[] { new DataPoint(DateTimeAxis.ToDouble(stockDate), (double) actualValue) }
                                         : new DataPoint[0])
                             .ToArray();

                if (values.Any())
                {
                    var series = new LineSeries();
                    series.Points.AddRange(values);
                    Wealth.Series.Add(series);
                }
            }

            Wealth.InvalidatePlot(true);
            Wealth.ResetAllAxes();
        }
    }
}