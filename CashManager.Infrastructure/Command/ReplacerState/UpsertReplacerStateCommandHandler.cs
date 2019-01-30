using CashManager.Infrastructure.Command.Parsers;
using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.ReplacerState
{
    public class UpsertReplacerStateCommandHandler : ICommandHandler<UpsertReplacerStateCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertReplacerStateCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertReplacerStateCommand command)
        {
            _db.Upsert(command.State);
        }
    }
}