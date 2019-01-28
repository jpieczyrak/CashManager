using CashManager.CommonData;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;

namespace CashManager.Features.Parsers.Custom
{
    public class RegexParserViewModel : ParserViewModelBase
    {
        protected RegexParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, TransactionsProvider transactionsProvider) : base(queryDispatcher,
            commandDispatcher, transactionsProvider) { }
    }
}