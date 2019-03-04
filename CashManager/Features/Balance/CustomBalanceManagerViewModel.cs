using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.CustomBalances;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.CustomBalances;
using CashManager.Infrastructure.Query.States;
using CashManager.Model.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoSearch = CashManager.Data.ViewModelState.SearchState;
using CustomBalance = CashManager.Logic.Balances.CustomBalance;
using DtoCustomBalance = CashManager.Data.ViewModelState.Balances.CustomBalance;
using SearchState = CashManager.Features.Search.SearchState;

namespace CashManager.Features.Balance
{
    public class CustomBalanceManagerViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public RelayCommand AddCustomBalanceCommand { get; }
        public RelayCommand<CustomBalance> RemoveCustomBalanceCommand { get; }

        public TrulyObservableCollection<CustomBalance> CustomBalances { get; private set; }

        public CustomBalanceManagerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;

            AddCustomBalanceCommand = new RelayCommand(() =>
            {
                var item = new CustomBalance(string.Empty);
                var query = new SearchStateQuery();
                item.SearchesPicker.SetInput(Mapper.Map<SearchState[]>(_queryDispatcher.Execute<SearchStateQuery, DtoSearch[]>(query)).Select(x => new Selectable(x)));
                CustomBalances.Add(item);
            });
            RemoveCustomBalanceCommand = new RelayCommand<CustomBalance>(ExecuteDeleteCommand);

            var customBalanceQuery = new CustomBalanceQuery();
            var customBalances = _queryDispatcher.Execute<CustomBalanceQuery, DtoCustomBalance[]>(customBalanceQuery);
            CustomBalances  = new TrulyObservableCollection<CustomBalance>(Mapper.Map<CustomBalance[]>(customBalances));
            CustomBalances.CollectionChanged += CustomBalancesOnCollectionChanged;
        }

        public void Update()
        {
            var query = new SearchStateQuery();
            var searches = Mapper.Map<SearchState[]>(_queryDispatcher.Execute<SearchStateQuery, DtoSearch[]>(query).OrderBy(x => x.InstanceCreationDate));
            foreach (var balance in CustomBalances)
            {
                var input = searches.Select(x => new Selectable(x)).ToArray();
                balance.SearchesPicker.SetInput(input, input.Where(x => balance.Searches.Contains(x.Value)));
            }
        }

        private void ExecuteDeleteCommand(CustomBalance selected)
        {
            _commandDispatcher.Execute(new DeleteCustomBalanceCommand(Mapper.Map<DtoCustomBalance>(selected)));
            CustomBalances.Remove(selected);
        }

        private void CustomBalancesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems == null) return;
            var changed = CustomBalances.Where(x => e.NewItems.Contains(x) && !string.IsNullOrWhiteSpace(x.Name));
            _commandDispatcher.Execute(new UpsertCustomBalanceCommand(Mapper.Map<DtoCustomBalance[]>(changed)));
        }
    }
}