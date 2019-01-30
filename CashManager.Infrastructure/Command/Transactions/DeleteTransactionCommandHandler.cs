using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Transactions
{
    public class DeleteTransactionCommandHandler : ICommandHandler<DeleteTransactionCommand>
    {
        private readonly LiteDatabase _db;

        public DeleteTransactionCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(DeleteTransactionCommand command)
        {
            _db.Remove(command.Transaction);
        }
    }
}