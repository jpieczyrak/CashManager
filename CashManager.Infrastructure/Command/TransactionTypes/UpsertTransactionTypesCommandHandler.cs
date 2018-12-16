using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.TransactionTypes
{
    public class UpsertTransactionTypesCommandHandler : ICommandHandler<UpsertTransactionTypesCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertTransactionTypesCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertTransactionTypesCommand command)
        {
            _db.UpsertBulk(command.Types);
        }
    }
}