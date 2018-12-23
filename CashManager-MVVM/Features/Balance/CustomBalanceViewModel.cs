using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.States;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Logic.Balances;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoSearch = CashManager.Data.ViewModelState.SearchState;

namespace CashManager_MVVM.Features.Balance
{
    public class CustomBalanceViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private CustomBalance _selectedCustomBalance;
        private Summary[] _selectedSearchSummary;
        private readonly SearchViewModel _searchViewModel;

        public CustomBalance SelectedCustomBalance
        {
            get => _selectedCustomBalance;
            set
            {
                Set(nameof(SelectedCustomBalance), ref _selectedCustomBalance, value);
                UpdateSummary();
            }
        }

        public Summary[] SelectedSearchSummary
        {
            get => _selectedSearchSummary;
            set => Set(nameof(SelectedSearchSummary), ref _selectedSearchSummary, value);
        }

        public CustomBalance[] CustomBalances { get; set; }
        
        public RelayCommand SaveCommand { get; private set; }

        public MultiComboBoxViewModel SavedSearches { get; private set; }

        public string Name { get; set; }

        public CustomBalanceViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory)
        {
            _searchViewModel = factory.Create<SearchViewModel>();
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            SelectedSearchSummary = new Summary[0];

            Name = "new custom balance";
            SelectedCustomBalance = new CustomBalance(Name);

            SavedSearches = new MultiComboBoxViewModel();
            SavedSearches.PropertyChanged += SavedSearchesOnPropertyChanged;

            Update();
        }

        private void SavedSearchesOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            SelectedCustomBalance.Searches = SavedSearches.Results.OfType<SearchState>().ToArray();
            UpdateSummary();
        }

        private void ExecuteSaveCommand()
        {
            //todo: save SelectedCustomBalance
        }

        public void Update()
        {
            var query = new SearchStateQuery();
            var source = Mapper.Map<SearchState[]>(_queryDispatcher.Execute<SearchStateQuery, DtoSearch[]>(query));

            SavedSearches.SetInput(source);
            //todo: custom balances load
        }

        private void UpdateSummary()
        {
            var summaries = new List<Summary>();
            foreach (var state in SelectedCustomBalance.Searches)
            {
                //todo: make it cleaner - do not use search vm?
                _searchViewModel.State.ApplySearchCriteria(state);
                _searchViewModel.Update();
                //todo: handle positions if needed
                var summary = _searchViewModel.TransactionsListViewModel.Summary.Copy();
                summary.Name = state.Name;
                summaries.Add(summary);
            }
            if (summaries.Any())
            {
                summaries.Add(new Summary
                {
                    Name = "Balance",
                    GrossIncome = summaries.Sum(x => x.GrossIncome),
                    GrossOutcome = summaries.Sum(x => x.GrossOutcome),
                    GrossBalance = summaries.Sum(x => x.GrossBalance)
                });
            }

            SelectedSearchSummary = summaries.ToArray();
        }
    }
}