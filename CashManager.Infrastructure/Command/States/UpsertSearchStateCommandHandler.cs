using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.States
{
    public class UpsertSearchStateCommandHandler : ICommandHandler<UpsertSearchState>
    {
        private readonly LiteDatabase _db;

        public UpsertSearchStateCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertSearchState command)
        {
            _db.Upsert(command.SearchState);
        }
    }
}