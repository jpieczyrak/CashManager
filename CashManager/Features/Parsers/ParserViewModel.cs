using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;
using CashManager.Logic.Parsers;
using CashManager.Logic.Parsers.Custom.Predefined;

using log4net;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager.Features.Parsers
{
    public class ParserViewModel : ParserViewModelBase
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(ParserViewModel)));

        protected KeyValuePair<string, IParser> _selectedParser;

        public Dictionary<string, IParser> Parsers { get; private set; }

        public KeyValuePair<string, IParser> SelectedParser
        {
            get => _selectedParser;
            set
            {
                Set(nameof(SelectedParser), ref _selectedParser, value);
                Parser = SelectedParser.Value;
            }
        }

        public ParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, TransactionsProvider transactionsProvider) : base(queryDispatcher,
            commandDispatcher, transactionsProvider)
        {
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
                { "Intelligo bank", new IntelligoBankParser() }
            };
            SelectedParser = Parsers.FirstOrDefault();
        }
    }
}