using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.States;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;

using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Logic.Commands;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.Search
{
    public class SearchViewModel : ViewModelBase, IUpdateable
    {
        #region fields

        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private Transaction[] _allTransactions;

        private Transaction[] _transactions;
        private Position[] _positions;

        private string _title;
        private bool _isTransactionsSearch;
        private bool _isPositionsSearch;
        
        private readonly TrulyObservableCollection<IFilter<Transaction>> _transactionFilters;
        private readonly TrulyObservableCollection<IFilter<Position>> _positionFilters;

        #endregion

        #region properties

        public TransactionListViewModel TransactionsListViewModel { get; }

        public PositionListViewModel PositionsListViewModel { get; }

        public SearchState State { get; }

        public Transaction[] Transactions
        {
            get => _transactions;
            set => Set(nameof(Transactions), ref _transactions, value);
        }

        public Position[] Positions
        {
            get => _positions;
            set => Set(nameof(Positions), ref _positions, value);
        }

        public string Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        public bool IsTransactionsSearch
        {
            get => _isTransactionsSearch;
            set
            {
                Set(nameof(IsTransactionsSearch), ref _isTransactionsSearch, value);
                if (value) SetTitle(SearchType.Transactions);
                FiltersOnCollectionChanged(null, null);
            }
        }

        public bool IsPositionsSearch
        {
            get => _isPositionsSearch;
            set
            {
                Set(nameof(IsPositionsSearch), ref _isPositionsSearch, value);
                if (value) SetTitle(SearchType.Positions);
                FiltersOnCollectionChanged(null, null);
            }
        }

        private bool CanExecuteAnyPositionFilter => _positionFilters.Any(x => x.CanExecute());

        private bool CanExecuteAnyTransactionFilter => _transactionFilters.Any(x => x.CanExecute());

        public RelayCommand SaveSearch { get; set; }

        #endregion

        public SearchViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ViewModelFactory factory)
        {
            State = new SearchState(queryDispatcher);
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;

            SaveSearch = new RelayCommand(ExecuteSaveSearchStateCommand);
            TransactionsListViewModel = factory.Create<TransactionListViewModel>();
            PositionsListViewModel = factory.Create<PositionListViewModel>();
            IsTransactionsSearch = true;

            var filters = new IFilter<Transaction>[]
            {
                DateFrameFilter.Create(State.BookDateFilter),
                DateFrameFilter.Create(State.CreateDateFilter),
                DateFrameFilter.Create(State.LastEditDateFilter),
                TextFilter.Create(State.TitleFilter),
                TextFilter.Create(State.NoteFilter),
                TextFilter.Create(State.PositionTitleFilter),
                MultiPickerFilter.Create(State.CategoriesFilter),
                MultiPickerFilter.Create(State.TagsFilter),
                MultiPickerFilter.Create(State.TypesFilter),
                MultiPickerFilter.Create(State.UserStocksFilter),
                MultiPickerFilter.Create(State.ExternalStocksFilter),
                RangeFilter.Create(State.ValueFilter)
            };
            _transactionFilters = new TrulyObservableCollection<IFilter<Transaction>>(filters);
            _positionFilters = new TrulyObservableCollection<IFilter<Position>>(filters.OfType<IFilter<Position>>());
            _transactionFilters.CollectionChanged += FiltersOnCollectionChanged;
            _positionFilters.CollectionChanged += FiltersOnCollectionChanged;

            Update();
        }

        private void ExecuteSaveSearchStateCommand()
        {
            _commandDispatcher.Execute(new UpsertSearchState(Mapper.Map<CashManager.Data.ViewModelState.SearchState>(State)));
        }

        public void Update()
        {
            _allTransactions = Mapper.Map<Transaction[]>(_queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()));
            Transactions = _allTransactions.ToArray();
            Positions = new Position[0];
            
            State.UpdateSources(_queryDispatcher);
            FiltersOnCollectionChanged(this, null);
        }

        private void FiltersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (_allTransactions == null || !_allTransactions.Any()) return;

            if (IsTransactionsSearch)
            {
                if (!CanExecuteAnyTransactionFilter)
                    if (TransactionsListViewModel.Transactions.Count == _allTransactions.Length) return;

                FilterTransactions();
            }
            else if (IsPositionsSearch)
            {
                if (!CanExecuteAnyPositionFilter)
                    if (PositionsListViewModel.Positions.Count == _allTransactions.Sum(x => x.Positions.Count)) return;
                FilterPositions();
            }
        }

        private void SetTitle(SearchType searchType)
        {
            Title = $"{searchType} search";
        }

        private void FilterTransactions()
        {
            var input = _allTransactions.AsEnumerable();
            foreach (var filter in _transactionFilters)
                if (filter.CanExecute())
                    input = filter.Execute(input);

            Transactions = OrderResults(input);
            TransactionsListViewModel.Transactions.Clear();
            TransactionsListViewModel.Transactions.AddRange(Transactions);
        }

        private void FilterPositions()
        {
            var input = _allTransactions.SelectMany(x => x.Positions);
            foreach (var filter in _positionFilters)
                if (filter.CanExecute())
                    input = filter.Execute(input);

            Positions = OrderResults(input);
            PositionsListViewModel.Positions.Clear();
            PositionsListViewModel.Positions.AddRange(Positions);
        }

        private static T[] OrderResults<T>(IEnumerable<T> input) where T : IBookable
        {
            return input
                   .OrderByDescending(x => x.BookDate)
                   .ThenByDescending(x => x.InstanceCreationDate)
                   .ToArray();
        }
    }
}