using AutoMapper;

using CashManager.CommonData;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;
using CashManager.Logic.Parsers;
using CashManager.Logic.Parsers.Custom;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager.Features.Parsers.Custom
{
    public class CsvParserViewModel : ParserViewModelBase
    {
        private TrulyObservableCollection<Model.Parsers.Rule> _rules;

        public TrulyObservableCollection<Model.Parsers.Rule> Rules
        {
            get => _rules;
            set
            {
                Set(ref _rules, value);
                Parser = new CustomCsvParser(Mapper.Map<Rule[]>(_rules), Mapper.Map<DtoStock[]>(UserStocks));
            }
        }

        public CsvParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, TransactionsProvider transactionsProvider) : base(queryDispatcher,
            commandDispatcher, transactionsProvider)
        {
            Parser = new RegexParser();
            Rules = new TrulyObservableCollection<Model.Parsers.Rule>();
        }
    }
}