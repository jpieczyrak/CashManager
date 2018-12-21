using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.States
{
    public class UpsertSearchStateCommandHandler : ICommandHandler<UpsertSearchStateCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertSearchStateCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertSearchStateCommand command)
        {
            _db.UpsertBulk(command.SearchStates);
        }
    }
}