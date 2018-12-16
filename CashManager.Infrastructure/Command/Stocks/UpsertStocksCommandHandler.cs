using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Stocks
{
    public class UpsertStocksCommandHandler : ICommandHandler<UpsertStocksCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertStocksCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertStocksCommand command)
        {
            _db.UpsertBulk(command.Stocks);
        }
    }
}