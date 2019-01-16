using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Plots
{
    public class FilterableViewModel : ViewModelBase, IUpdateable
    {
        protected readonly TransactionsProvider _transactionsProvider;
        protected readonly IQueryDispatcher _queryDispatcher;

        private DateFrame _bookDateFilter;
        private MultiPicker _userStocksFilter;

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

        protected IEnumerable<Transaction> TransactionsMatchingUserStock
        {
            get
            {
                var stockHashSet = new HashSet<Stock>(UserStocksFilter.Results.Select(x => x.Value as Stock));
                return _transactionsProvider.AllTransactions
                                            .Where(x => !UserStocksFilter.IsChecked || stockHashSet.Contains(x.UserStock));
            }
        }

        protected IEnumerable<Transaction> MatchingTransactions
        {
            get
            {
                return TransactionsMatchingUserStock
                    .Where(x => !BookDateFilter.IsChecked
                                || x.BookDate >= BookDateFilter.From
                                && x.BookDate <= BookDateFilter.To);
            }
        }

        protected FilterableViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider transactionsProvider)
        {
            _queryDispatcher = queryDispatcher;
            _transactionsProvider = transactionsProvider;

            var dtos = _queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery());
            var stocks = Mapper.Map<Stock[]>(dtos.Where(x => x.IsUserStock))
                               .OrderBy(x => x.Name)
                               .Select(x => new Selectable(x))
                               .ToArray();
            UserStocksFilter = new MultiPicker(MultiPickerType.UserStock, stocks) { IsChecked = true };
            foreach (var result in UserStocksFilter.ComboBox.InternalDisplayableSearchResults) result.IsSelected = true;

            _bookDateFilter = new DateFrame(DateFrameType.BookDate) { IsChecked = true };
            BookDateFilter.From = _transactionsProvider.AllTransactions.Any()
                                      ? _transactionsProvider.AllTransactions.Min(x => x.BookDate).AddDays(-1)
                                      : DateTime.MinValue;
            BookDateFilter.To = _transactionsProvider.AllTransactions.Any()
                                    ? _transactionsProvider.AllTransactions.Max(x => x.BookDate)
                                    : DateTime.MaxValue;

            BookDateFilter.PropertyChanged += OnPropertyChanged;
            UserStocksFilter.PropertyChanged += (sender, args) =>
            {
                UpdateDateFilterRanges();
                OnPropertyChanged(sender, args);
            };
        }

        #region IUpdateable

        public virtual void Update()
        {
            var dtos = _queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery());
            var stocks = Mapper.Map<Stock[]>(dtos.Where(x => x.IsUserStock))
                               .OrderBy(x => x.Name)
                               .Select(x => new Selectable(x))
                               .ToArray();

            UserStocksFilter.SetInput(stocks);

            OnPropertyChanged(this, null);
        }

        #endregion

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) { }

        private void UpdateDateFilterRanges()
        {
            if (!TransactionsMatchingUserStock.Any()) return;

            BookDateFilter.From = TransactionsMatchingUserStock.Min(x => x.BookDate).AddDays(-1);
            BookDateFilter.To = TransactionsMatchingUserStock.Max(x => x.BookDate);
        }
    }
}