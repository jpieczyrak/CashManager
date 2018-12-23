using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.States;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Logic.Balances;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoSearch = CashManager.Data.ViewModelState.SearchState;

namespace CashManager_MVVM.Features.Balance
{
    public class CustomBalanceViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public CustomBalance SelectedCustomBalance { get; set; }

        public CustomBalance[] CustomBalances { get; set; }
        
        public RelayCommand SaveCommand { get; private set; }

        public MultiComboBoxViewModel SavedSearches { get; private set; }

        public string Name { get; set; }

        public CustomBalanceViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            SaveCommand = new RelayCommand(ExecuteSaveCommand);

            SavedSearches = new MultiComboBoxViewModel();

            Update();
        }

        private void ExecuteSaveCommand()
        {
            var balance = new CustomBalance(Name) { Searches = SavedSearches.Results.OfType<SearchState>().ToArray() };
            //todo: save
        }

        public void Update()
        {
            var query = new SearchStateQuery();
            var source = Mapper.Map<SearchState[]>(_queryDispatcher.Execute<SearchStateQuery, DtoSearch[]>(query));

            SavedSearches.SetInput(source);
            //todo: custom balances load
        }
    }
}