using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Features.Transactions;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Logic.Parsers;
using CashManager.Logic.Parsers.Custom.Predefined;
using CashManager.Logic.Wrappers;
using CashManager.Messages.Models;
using CashManager.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using GongSolutions.Wpf.DragDrop;

using log4net;

using DtoStock = CashManager.Data.DTO.Stock;
using DtoBalance = CashManager.Data.DTO.Balance;
using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoTransactionType = CashManager.Data.DTO.TransactionType;

namespace CashManager.Features.Parsers
{
    public class ParserViewModel : ViewModelBase, IUpdateable, IDropTarget
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(ParserViewModel)));
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private string _inputText;

        private bool _generateMissingStocks;

        private TransactionListViewModel _resultsListViewModel = new TransactionListViewModel();
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

        public bool GenerateMissingStocks
        {
            get => _generateMissingStocks;
            set => Set(ref _generateMissingStocks, value);
        }

        public TransactionListViewModel ResultsListViewModel
        {
            get => _resultsListViewModel;
            private set => Set(ref _resultsListViewModel, value, nameof(ResultsListViewModel));
        }

        public TransactionsProvider TransactionsProvider { get; }

        public ParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, TransactionsProvider transactionsProvider)
        {
            TransactionsProvider = transactionsProvider;
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;

            Update();

            //todo: refresh stocks
            var factory = new CustomCsvParserFactory(Mapper.Map<DtoStock[]>(UserStocks.Concat(ExternalStocks)));
            Parsers = new Dictionary<string, IParser>
            {
                { "Getin bank", new GetinBankParser() },
                { "Idea bank", new IdeaBankParser() },
                { "Millennium bank", new MillenniumBankParser() },
                { "Millennium bank (csv)", factory.Create(PredefinedCsvParsers.Millennium) },
                { "Ing bank (web)", new IngBankParser() },
                { "Ing bank (csv)", factory.Create(PredefinedCsvParsers.Ing) },
                { "Intelligo bank", new IntelligoBankParser() },
                { "Excel", new ExcelParser() }
            };
            SelectedParser = Parsers.FirstOrDefault();

            ParseCommand = new RelayCommand(ExecuteParseCommand, CanExecuteParseCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
        }

        private bool CanExecuteParseCommand() => !string.IsNullOrEmpty(InputText);

        private void ExecuteParseCommand()
        {
            var parser = SelectedParser.Value;
            DtoTransaction[] results = null;
            IEnumerable<Transaction> transactions = null;
            using (new MeasureTimeWrapper(
                () => results = parser.Parse(InputText, Mapper.Map<DtoStock>(SelectedUserStock),
                          Mapper.Map<DtoStock>(SelectedExternalStock),
                          Mapper.Map<DtoTransactionType>(DefaultOutcomeTransactionType),
                          Mapper.Map<DtoTransactionType>(DefaultIncomeTransactionType), GenerateMissingStocks), $"Parsing: {SelectedParser.Key}")) { }
            using (new MeasureTimeWrapper(
                () => transactions = Mapper.Map<Transaction[]>(results).Where(x => x.IsValid), $"Mapping: {results.Length,6}")) { }
            using (new MeasureTimeWrapper(
                () => ResultsListViewModel = new TransactionListViewModel
                {
                    Transactions = new TrulyObservableCollection<Transaction>(transactions)
                }, "List creation")) { }


            RaisePropertyChanged(nameof(ResultsListViewModel));
        }

        private bool CanExecuteSaveCommand() => ResultsListViewModel != null && ResultsListViewModel.Transactions.Any();

        private void ExecuteSaveCommand()
        {
            var transactions = ResultsListViewModel.Transactions;
            _commandDispatcher.Execute(new UpsertTransactionsCommand(Mapper.Map<DtoTransaction[]>(transactions)));

            TransactionsProvider.AllTransactions.RemoveRange(transactions);
            TransactionsProvider.AllTransactions.AddRange(transactions);

            var balances = SelectedParser.Value.Balances.ToArray();
            if (balances.Any()) //todo: ask if balances should be updated
            {
                var updatedStocks = Mapper.Map<Stock[]>(balances.Select(x => x.Key));
                var updatedDtos = UpdateStockBalances(balances, updatedStocks);
                _commandDispatcher.Execute(new UpsertStocksCommand(updatedDtos));
                MessengerInstance.Send(new UpdateStockMessage(updatedStocks));
            }
        }

        private static DtoStock[] UpdateStockBalances(KeyValuePair<DtoStock, DtoBalance>[] balances, Stock[] updatedStocks)
        {
            var idBalances = balances.ToDictionary(x => x.Key.Id, x => x.Value);

            foreach (var stock in updatedStocks)
            {
                stock.Balance.IsPropertyChangedEnabled = true;
                stock.Balance.Value = idBalances[stock.Id].Value;
            }

            return Mapper.Map<DtoStock[]>(updatedStocks);
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
            IncomeTransactionTypes = types
                                     .Where(x => x.Income)
                                     .OrderBy(x => !x.IsDefault)
                                     .ThenBy(x => x.InstanceCreationDate)
                                     .ToArray();
            OutcomeTransactionTypes = types
                                      .Where(x => x.Outcome)
                                      .OrderBy(x => !x.IsDefault)
                                      .ThenBy(x => x.InstanceCreationDate)
                                      .ToArray();
            DefaultIncomeTransactionType = IncomeTransactionTypes.FirstOrDefault();
            DefaultOutcomeTransactionType = OutcomeTransactionTypes.FirstOrDefault();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = GetProperFilepaths(dropInfo).Any()
                                   ? DragDropEffects.Copy
                                   : DragDropEffects.None;
        }


        public void Drop(IDropInfo dropInfo)
        {
            string input = string.Empty;
            var files = GetProperFilepaths(dropInfo);
            foreach (string file in files)
                if (File.Exists(file))
                    input += File.ReadAllText(file, Encoding.Default);

            InputText = input;
        }

        private string[] GetProperFilepaths(IDropInfo dropInfo)
        {
            string[] allowedExtensions = { ".csv", ".txt" };
            return ((DataObject)dropInfo.Data).GetFileDropList()
                                              .OfType<string>()
                                              .Where(x => !string.IsNullOrWhiteSpace(x))
                                              .Where(x => allowedExtensions.Contains(Path.GetExtension(x).ToLower()))
                                              .ToArray();
        }
    }
}