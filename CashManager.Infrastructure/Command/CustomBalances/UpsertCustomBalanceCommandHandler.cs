using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.CustomBalances
{
    public class UpsertCustomBalanceCommandHandler : ICommandHandler<UpsertCustomBalanceCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertCustomBalanceCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertCustomBalanceCommand command)
        {
            _db.UpsertBulk(command.CustomBalances);
        }
    }
}