using System;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Plots
{
    public class FilterableViewModel : ViewModelBase, IUpdateable
    {
        protected readonly TransactionsProvider _transactionsProvider;
        protected readonly IQueryDispatcher _queryDispatcher;

        protected DateFrame _bookDateFilter;
        protected MultiPicker _userStocksFilter;

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

        protected FilterableViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider transactionsProvider)
        {
            _queryDispatcher = queryDispatcher;
            _transactionsProvider = transactionsProvider;
            _bookDateFilter = new DateFrame(DateFrameType.BookDate);
        }

        #region IUpdateable

        public virtual void Update()
        {
            var dtos = _queryDispatcher.Execute<StockQuery, Stock[]>(new StockQuery());
            var stocks = Mapper.Map<Model.Stock[]>(dtos)
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

        #endregion

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) { }
    }
}