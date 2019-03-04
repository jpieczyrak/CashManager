using CashManager.CommonData;
using CashManager.Features.MassReplacer;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;
using CashManager.Logic.Creators;
using CashManager.Logic.Parsers;

namespace CashManager.Features.Parsers.Custom
{
    public class RegexParserViewModel : ParserViewModelBase
    {
        private string _regexValue;

        public string RegexValue
        {
            get => _regexValue;
            set
            {
                Set(ref _regexValue, value);
                (Parser as RegexParser).RegexValue = value;
            }
        }

        public RegexParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ICorrectionsCreator correctionsCreator, TransactionsProvider transactionsProvider, MassReplacerViewModel replacer) : base(queryDispatcher,
            commandDispatcher, correctionsCreator, transactionsProvider, replacer)
        {
            Parser = new RegexParser();
        }
    }
}