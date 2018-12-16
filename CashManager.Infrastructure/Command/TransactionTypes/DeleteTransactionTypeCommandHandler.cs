using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.TransactionTypes
{
    public class DeleteTransactionTypeCommandHandler : ICommandHandler<DeleteTransactionTypeCommand>
    {
        private readonly LiteDatabase _db;

        public DeleteTransactionTypeCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(DeleteTransactionTypeCommand command)
        {
            _db.Remove(command.TransactionType);
        }
    }
}