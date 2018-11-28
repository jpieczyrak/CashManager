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
            if (command.Transactions != null)
            {
                var positions = command.Transactions.SelectMany(x => x.Positions).ToArray();
                _db.UpsertBulk(positions);

                _db.UpsertBulk(positions.Select(x => x.Category).ToArray());
                _db.UpsertBulk(positions.Where(x => x.Tags != null).SelectMany(x => x.Tags).ToArray());
            }
        }
    }
}