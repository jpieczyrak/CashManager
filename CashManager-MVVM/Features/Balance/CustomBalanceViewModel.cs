using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.CustomBalances;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.CustomBalances;
using CashManager.Infrastructure.Query.States;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Logic.Balances;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoSearch = CashManager.Data.ViewModelState.SearchState;
using DtoCustomBalance = CashManager.Data.ViewModelState.Balances.CustomBalance;

namespace CashManager_MVVM.Features.Balance
{
    public class CustomBalanceViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private CustomBalance _selectedCustomBalance;
        private TransactionsSummary[] _selectedSearchSummary;
        private readonly SearchViewModel _searchViewModel;
        private string _name;
        private ObservableCollection<CustomBalance> _customBalances;
        private DateFrame _dateFilter;

        public CustomBalance SelectedCustomBalance
        {
            get => _selectedCustomBalance;
            set
            {
                Set(nameof(SelectedCustomBalance), ref _selectedCustomBalance, value);
                UpdateSummary();
                UpdateSelectedSearches();
            }
        }

        private void UpdateSelectedSearches()
        {
            if (SavedSearches != null)
                foreach (var result in SavedSearches.InternalDisplayableSearchResults)
                    result.IsSelected = SelectedCustomBalance.Searches.Contains(result);
            SavedSearches?.RaisePropertyChanged();
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

        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand DeleteCommand { get; private set; }

        public MultiComboBoxViewModel SavedSearches { get; private set; }

        public string Name
        {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }

        public DateFrame DateFilter
        {
            get => _dateFilter;
            set => Set(nameof(DateFilter), ref _dateFilter, value);
        }

        public CustomBalanceViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory)
        {
            _searchViewModel = factory.Create<SearchViewModel>();
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand, () => SelectedCustomBalance != null);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            SelectedSearchSummary = new TransactionsSummary[0];
            DateFilter = new DateFrame(DateFrameType.BookDate);
            DateFilter.PropertyChanged += (sender, args) => UpdateSummary();

            Name = "custom balance";
            _selectedCustomBalance = new CustomBalance(Name);

            SavedSearches = new MultiComboBoxViewModel();
            SavedSearches.PropertyChanged += SavedSearchesOnPropertyChanged;

            var customBalanceQuery = new CustomBalanceQuery();
            var customBalances = _queryDispatcher.Execute<CustomBalanceQuery, DtoCustomBalance[]>(customBalanceQuery);
            CustomBalances = Mapper.Map<ObservableCollection<CustomBalance>>(customBalances);
        }

        private void SavedSearchesOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            SelectedCustomBalance.Searches = SavedSearches.Results.OfType<SearchState>().ToArray();
            UpdateSummary();
        }

        private void ExecuteSaveCommand()
        {
            var balance = new CustomBalance(Name) { Searches = SelectedCustomBalance.Searches };
            _commandDispatcher.Execute(new UpsertCustomBalanceCommand(Mapper.Map<DtoCustomBalance>(balance)));
            CustomBalances.Add(balance);
            SelectedCustomBalance = balance;
        }

        private void ExecuteDeleteCommand()
        {
            _commandDispatcher.Execute(new DeleteCustomBalanceCommand(Mapper.Map<DtoCustomBalance>(SelectedCustomBalance)));
            CustomBalances.Remove(SelectedCustomBalance);
            if (CustomBalances.Any()) SelectedCustomBalance = CustomBalances.First();
        }

        public void Update()
        {
            var query = new SearchStateQuery();
            var source = Mapper.Map<SearchState[]>(_queryDispatcher.Execute<SearchStateQuery, DtoSearch[]>(query));
            SavedSearches.SetInput(source);
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