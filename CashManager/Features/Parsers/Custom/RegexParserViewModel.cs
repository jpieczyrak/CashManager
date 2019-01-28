using CashManager.CommonData;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;
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

        public RegexParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, TransactionsProvider transactionsProvider) : base(queryDispatcher,
            commandDispatcher, transactionsProvider)
        {
            Parser = new RegexParser();
        }
    }
}