using CashManager.Infrastructure.Command.States;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Parsers
{
    public class UpsertCsvParserCommandHandler : ICommandHandler<UpsertCsvParserCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertCsvParserCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertCsvParserCommand command)
        {
            _db.Upsert(command.Parser);
        }
    }
}