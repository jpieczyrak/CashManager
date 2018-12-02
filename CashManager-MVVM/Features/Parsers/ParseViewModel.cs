using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Logic.Parsers;

using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Messages;
using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoStock = CashManager.Data.DTO.Stock;
using DtoTransactionType = CashManager.Data.DTO.TransactionType;
using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Features.Parsers
{
    public class ParseViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private string _inputText;

        private TransactionListViewModel _resultsListViewModel;
        private KeyValuePair<string, IParser> _selectedParser;

        public string InputText
        {
            get => _inputText;
            set => Set(nameof(InputText), ref _inputText, value);
        }

        public Dictionary<string, IParser> Parsers { get; private set; }

        public KeyValuePair<string, IParser> SelectedParser
        {
            get => _selectedParser;
            set => Set(nameof(SelectedParser), ref _selectedParser, value);
        }

        public Stock SelectedUserStock { get; set; }

        public Stock[] UserStocks { get; set; }

        public Stock SelectedExternalStock { get; set; }

        public Stock[] ExternalStocks { get; set; }

        public TransactionType DefaultIncomeTransactionType { get; set; }

        public TransactionType DefaultOutcomeTransactionType { get; set; }

        public TransactionType[] IncomeTransactionTypes { get; set; }

        public TransactionType[] OutcomeTransactionTypes { get; set; }

        public RelayCommand ParseCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public TransactionListViewModel ResultsListViewModel
        {
            get => _resultsListViewModel;
            private set => Set(ref _resultsListViewModel, value, nameof(ResultsListViewModel));
        }

        public ParseViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;

            Parsers = new Dictionary<string, IParser>
            {
                { "Getin bank", new GetinBankParser() },
                { "Idea bank", new IdeaBankParser() },
                { "Millennium bank", new MillenniumBankParser() },
                { "Ing bank", new IngBankParser() },
                { "Excel", new ExcelParser() }
            };
            SelectedParser = Parsers.FirstOrDefault();

            ParseCommand = new RelayCommand(ExecuteParseCommand, CanExecuteParseCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
        }

        private bool CanExecuteSaveCommand()
        {
            return ResultsListViewModel != null && ResultsListViewModel.Transactions.Any();
        }

        private void ExecuteSaveCommand()
        {
            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<DtoTransaction[]>(ResultsListViewModel.Transactions)));
            var balance = SelectedParser.Value.Balance;
            if (balance != null)
            {
                if (SelectedUserStock.Balance == null || balance.Date > SelectedUserStock.Balance.Date)
                {
                    SelectedUserStock.Balance = Mapper.Map<Balance>(balance);
                    _commandDispatcher.Execute(new UpsertStocksCommand(Mapper.Map<DtoStock[]>(new [] { SelectedUserStock } )));
                    MessengerInstance.Send(new StockUpdateMessage(SelectedUserStock));
                }
            }
        }

        private bool CanExecuteParseCommand()
        {
            return !string.IsNullOrEmpty(InputText);
        }

        private void ExecuteParseCommand()
        {
            var transactions = Mapper.Map<Transaction[]>(SelectedParser.Value.Parse(InputText, Mapper.Map<DtoStock>(SelectedUserStock),
                Mapper.Map<DtoStock>(SelectedExternalStock),
                Mapper.Map<DtoTransactionType>(DefaultOutcomeTransactionType),
                Mapper.Map<DtoTransactionType>(DefaultIncomeTransactionType)));

            ResultsListViewModel = new TransactionListViewModel { Transactions = new TrulyObservableCollection<Transaction>(transactions) };
            RaisePropertyChanged(nameof(ResultsListViewModel));
        }

        public void Update()
        {
            var stocks = Mapper.Map<Stock[]>(_queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()));
            UserStocks = stocks.Where(x => x.IsUserStock)
                               .OrderBy(x => x.InstanceCreationDate)
                               .ToArray();
            ExternalStocks = stocks.Where(x => !x.IsUserStock)
                                   .OrderBy(x => x.InstanceCreationDate)
                                   .ToArray();
            SelectedUserStock = UserStocks.FirstOrDefault();
            SelectedExternalStock = ExternalStocks.FirstOrDefault();

            var types = Mapper.Map<TransactionType[]>(
                _queryDispatcher.Execute<TransactionTypesQuery, DtoTransactionType[]>(new TransactionTypesQuery()))
                              .OrderBy(x => x.InstanceCreationDate)
                              .ToArray();
            IncomeTransactionTypes = types.Where(x => x.Income).OrderBy(x => x.IsDefault).ThenBy(x => x.InstanceCreationDate).ToArray();
            OutcomeTransactionTypes = types.Where(x => x.Outcome).OrderBy(x => x.IsDefault).ThenBy(x => x.InstanceCreationDate).ToArray();
            DefaultIncomeTransactionType = IncomeTransactionTypes.FirstOrDefault();
            DefaultOutcomeTransactionType = OutcomeTransactionTypes.FirstOrDefault();
        }
    }
}