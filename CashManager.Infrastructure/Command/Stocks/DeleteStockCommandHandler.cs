using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Stocks
{
    public class DeleteStockCommandHandler : ICommandHandler<DeleteStockCommand>
    {
        private readonly LiteDatabase _db;

        public DeleteStockCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(DeleteStockCommand command)
        {
            _db.Remove(command.Stock);
        }
    }
}