using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AutoMapper;

using CashManager.Features.Search;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.CustomBalances;
using CashManager.Logic.Balances;
using CashManager.Model;
using CashManager.Model.Selectors;

using GalaSoft.MvvmLight;

using DtoCustomBalance = CashManager.Data.ViewModelState.Balances.CustomBalance;

namespace CashManager.Features.Balance
{
    public class CustomBalanceViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly SearchViewModel _searchViewModel;
        private TransactionsSummary[] _selectedSearchSummary;
        private CustomBalance _selectedCustomBalance;
        private ObservableCollection<CustomBalance> _customBalances;
        private DateFrameSelector _dateFilter;

        public CustomBalance SelectedCustomBalance
        {
            get => _selectedCustomBalance;
            set
            {
                Set(nameof(SelectedCustomBalance), ref _selectedCustomBalance, value);
                UpdateSummary();
            }
        }

        public TransactionsSummary[] SelectedSearchSummary
        {
            get => _selectedSearchSummary;
            set => Set(nameof(SelectedSearchSummary), ref _selectedSearchSummary, value);
        }

        public ObservableCollection<CustomBalance> CustomBalances
        {
            get => _customBalances;
            set => Set(nameof(CustomBalances), ref _customBalances, value);
        }

        public DateFrameSelector DateFilter
        {
            get => _dateFilter;
            set => Set(nameof(DateFilter), ref _dateFilter, value);
        }

        public CustomBalanceViewModel(IQueryDispatcher queryDispatcher, ViewModelFactory factory)
        {
            _searchViewModel = factory.Create<SearchViewModel>();
            _queryDispatcher = queryDispatcher;
            SelectedSearchSummary = new TransactionsSummary[0];
            DateFilter = new DateFrameSelector(DateFrameType.BookDate);
            DateFilter.PropertyChanged += (sender, args) => UpdateSummary();
        }

        public void Update()
        {
            var customBalanceQuery = new CustomBalanceQuery();
            var customBalances = _queryDispatcher.Execute<CustomBalanceQuery, DtoCustomBalance[]>(customBalanceQuery);
            CustomBalances = Mapper.Map<ObservableCollection<CustomBalance>>(customBalances);
        }

        private void UpdateSummary()
        {
            var summaries = new List<TransactionsSummary>();
            if (SelectedCustomBalance != null)
            {
                _searchViewModel.Update();
                foreach (var state in SelectedCustomBalance.Searches)
                {
                    if (DateFilter.IsChecked) state.BookDateFilter.Apply(DateFilter);
                    else
                    {
                        state.BookDateFilter.From = DateTime.MinValue;
                        state.BookDateFilter.To = DateTime.MaxValue;
                    }
                    //todo: make it cleaner - do not use search vm?
                    _searchViewModel.State.ApplySearchCriteria(state);
                    _searchViewModel.PerformFilter(_searchViewModel.Provider.AllTransactions);
                    //todo: handle positions if needed
                    var summary = _searchViewModel.TransactionsListViewModel.Summary.Copy();
                    summary.Name = state.Name;
                    summaries.Add(summary);
                }
            }

            if (summaries.Any())
            {
                summaries.Add(new TransactionsSummary
                {
                    Name = "Balance",
                    GrossIncome = summaries.Sum(x => x.GrossIncome),
                    GrossOutcome = summaries.Sum(x => x.GrossOutcome),
                    GrossBalance = summaries.Sum(x => x.GrossBalance),
                    IncomesCount = summaries.Sum(x => x.IncomesCount),
                    OutcomesCount = summaries.Sum(x => x.OutcomesCount),
                });
            }

            SelectedSearchSummary = summaries.ToArray();
        }
    }
}