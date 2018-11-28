using System.Linq;

using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Transactions
{
    public class UpsertTransactionsCommandHandler : ICommandHandler<UpsertTransactionsCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertTransactionsCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertTransactionsCommand command)
        {
            _db.UpsertBulk(command.Transactions);
            var positions = command.Transactions.SelectMany(x => x.Positions).ToArray();
            _db.UpsertBulk(positions);
        }
    }
}