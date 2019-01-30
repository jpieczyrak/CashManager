using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Model.Selectors;

using GalaSoft.MvvmLight;

using OxyPlot;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoTransactionType = CashManager.Data.DTO.TransactionType;

namespace CashManager.Features.Plots
{
    public class CategoriesPlotViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly TransactionsProvider _transactionsProvider;
        private DateFrameSelector _bookDateFilter = new DateFrameSelector(DateFrameType.BookDate);
        private IEnumerable<KeyValuePair<string, decimal>> _values;
        private MultiPicker _userStocksFilter;
        private PlotModel _pieCategories;
        private MultiPicker _typesFilter;
        private bool _topCategoriesOnly = true;
        private Dictionary<Guid, Category> _allCategories;

        public DateFrameSelector BookDateFilter
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

        public PlotModel PieCategories
        {
            get => _pieCategories;
            set => Set(nameof(PieCategories), ref _pieCategories, value);
        }

        public IEnumerable<KeyValuePair<string, decimal>> Values
        {
            get => _values;
            private set => Set(ref _values, value);
        }

        public bool TopCategoriesOnly
        {
            get => _topCategoriesOnly;
            set
            {
                Set(ref _topCategoriesOnly, value);
                OnPropertyChanged(null, null);
            }
        }

        public CategoriesPlotViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider transactionsProvider)
        {
            _queryDispatcher = queryDispatcher;
            _transactionsProvider = transactionsProvider;
            PieCategories = PlotHelper.CreatePlotModel();
        }

        #region IUpdateable

        public void Update()
        {
            var stocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()))
                               .Where(x => x.IsUserStock)
                               .OrderBy(x => x.Name)
                               .ToArray();
            UserStocksFilter = new MultiPicker(MultiPickerType.UserStock, stocks.Select(x => new Selectable(x)).ToArray());
            foreach (var result in UserStocksFilter.ComboBox.InternalDisplayableSearchResults) result.IsSelected = true;
            UserStocksFilter.IsChecked = true;
            UserStocksFilter.PropertyChanged += OnPropertyChanged;

            BookDateFilter.From = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            BookDateFilter.To = BookDateFilter.From.AddMonths(1).AddDays(-1);
            BookDateFilter.IsChecked = true;
            BookDateFilter.PropertyChanged += OnPropertyChanged;

            var types = Mapper
                        .Map<TransactionType[]>(_queryDispatcher.Execute<TransactionTypesQuery, DtoTransactionType[]>(new TransactionTypesQuery())
                                                                .OrderBy(x => !x.Outcome)
                                                                .ThenBy(x => x.Name))
                        .Select(x => new Selectable(x))
                        .ToArray();
            TypesFilter = new MultiPicker(MultiPickerType.TransactionType, types);
            foreach (var x in TypesFilter.ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = ((TransactionType) x.Value).Outcome;
            TypesFilter.IsChecked = true;
            TypesFilter.PropertyChanged += OnPropertyChanged;

            _allCategories = Mapper.Map<Category[]>(_queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery())).ToDictionary(x => x.Id, x => x);

            OnPropertyChanged(this, null);
        }

        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            PieCategories.Series.Clear();
            var transactions = _transactionsProvider.AllTransactions;
            if (transactions == null || !transactions.Any()) return;

            var selectedStocks = UserStocksFilter.IsChecked
                                     ? UserStocksFilter.Results.Select(x => x.Value as Stock).ToArray()
                                     : null;
            var selectedTypes = new HashSet<TransactionType>(TypesFilter.Results.Select(x => x.Value as TransactionType));

            if (selectedStocks != null && selectedStocks.Any())
            {
                var positions = transactions
                                 .Where(x => selectedStocks.Contains(x.UserStock))
                                 .Where(x => !BookDateFilter.IsChecked || x.BookDate >= BookDateFilter.From && x.BookDate <= BookDateFilter.To)
                                 .Where(x => !TypesFilter.IsChecked || selectedTypes.Contains(x.Type))
                                 .SelectMany(x => x.Positions)
                                 .Where(x => x.Category != null);
                var groups = TopCategoriesOnly
                                         ? positions.GroupBy(x => GetTopParentName(x.Category))
                                         : positions.Where(x => _allCategories.All(y => y.Value.Parent?.Id != x.Id))
                                                    .GroupBy(x => x.Category.Name);

                Values = groups
                         .Select(x =>
                         {
                             decimal value = x.Sum(y => y.Value.GrossValue);
                             return new KeyValuePair<string, decimal>(x.Key, value);
                         })
                         .OrderByDescending(x => x.Value)
                         .ToArray();

                if (Values.Any())
                {
                    var series = new PieSeries();
                    foreach (var value in Values) series.Slices.Add(new PieSlice(value.Key, (double) value.Value));

                    PieCategories.Series.Add(series);
                }
            }

            PieCategories.InvalidatePlot(true);
        }

        private string GetTopParentName(Category category)
        {
            if (category.Parent != null) return GetTopParentName(_allCategories[category.Parent.Id]);
            return category.Name;
        }
    }
}