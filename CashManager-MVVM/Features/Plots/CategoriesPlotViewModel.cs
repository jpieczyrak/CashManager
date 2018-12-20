using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

using OxyPlot;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;
using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.Plots
{
    public class CategoriesPlotViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private Transaction[] _allTransactions;
        private DateFrame _bookDateFilter = new DateFrame(DateFrameType.BookDate);
        private MultiPicker _userStocksFilter;
        private PlotModel _columnCategories;
        private PlotModel _pieCategories;
        private MultiPicker _typesFilter;

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
        public MultiPicker TypesFilter
        {
            get => _typesFilter;
            set => Set(nameof(TypesFilter), ref _typesFilter, value);
        }

        public PlotModel ColumnCategories
        {
            get => _columnCategories;
            set => Set(nameof(ColumnCategories), ref _columnCategories, value);
        }

        public PlotModel PieCategories
        {
            get => _pieCategories;
            set => Set(nameof(PieCategories), ref _pieCategories, value);
        }

        public CategoriesPlotViewModel(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            ColumnCategories = PlotHelper.CreatePlotModel();
            PieCategories = PlotHelper.CreatePlotModel();
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

            BookDateFilter.From = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            BookDateFilter.To = BookDateFilter.From.AddMonths(1).AddDays(-1);
            BookDateFilter.IsChecked = true;
            BookDateFilter.PropertyChanged += OnPropertyChanged;

            var types = Mapper.Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, CashManager.Data.DTO.TransactionType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesFilter = new MultiPicker("Types", types);
            foreach (var x in TypesFilter.ComboBox.InternalDisplayableSearchResults.OfType<TransactionType>())
                x.IsSelected = x.Outcome;
            TypesFilter.IsChecked = true;
            TypesFilter.PropertyChanged += OnPropertyChanged;

            OnPropertyChanged(this, null);
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            ColumnCategories.Series.Clear();
            PieCategories.Series.Clear();
            if (_allTransactions == null || !_allTransactions.Any()) return;

            var selectedStocks = UserStocksFilter.IsChecked
                                     ? UserStocksFilter.Results.OfType<Stock>().ToArray()
                                     : null;
            var selectedTypes = new HashSet<TransactionType>(TypesFilter.Results.OfType<TransactionType>());

            if (selectedStocks != null && selectedStocks.Any())
            {
                var values = _allTransactions
                             .Where(x => selectedStocks.Contains(x.UserStock))
                             .Where(x => !BookDateFilter.IsChecked || x.BookDate >= BookDateFilter.From && x.BookDate <= BookDateFilter.To)
                             .Where(x => !TypesFilter.IsChecked || selectedTypes.Contains(x.Type))
                             .SelectMany(x => x.Positions)
                             .Where(x => x.Category != null)
                             .GroupBy(x => x.Category.Name)
                             .Select(x =>
                             {
                                 decimal value = x.Sum(y => y.Value.GrossValue);
                                 return new { Title = x.Key, Value = value} ;
                             })
                             .OrderByDescending(x => x.Value)
                             .ToArray();

                if (values.Any())
                {
                    var series = new PieSeries();
                    foreach (var value in values)
                    {
                        ColumnCategories.Series.Add(new ColumnSeries
                        {
                            Title = value.Title,
                            ItemsSource = new [] { value },
                            ValueField = nameof(value.Value)
                        });
                        series.Slices.Add(new PieSlice(value.Title, (double)value.Value));
                    }
                    PieCategories.Series.Add(series);
                }
            }

            ColumnCategories.InvalidatePlot(true);
            ColumnCategories.ResetAllAxes();

            PieCategories.InvalidatePlot(true);
        }
    }
}