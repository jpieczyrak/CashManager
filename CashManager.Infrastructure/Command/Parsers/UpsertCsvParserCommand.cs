using CashManager.Data.ViewModelState.Parsers;

namespace CashManager.Infrastructure.Command.Parsers
{
    public class UpsertCsvParserCommand : ICommand
    {
        public CustomCsvParser Parser { get; }

        public UpsertCsvParserCommand(CustomCsvParser parser)
        {
            Parser = parser;
        }
    }
}