using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.CustomBalances
{
    public class DeleteCustomBalanceCommandHandler : ICommandHandler<DeleteCustomBalanceCommand>
    {
        private readonly LiteDatabase _db;

        public DeleteCustomBalanceCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(DeleteCustomBalanceCommand command)
        {
            _db.Remove(command.CustomBalance);
        }
    }
}