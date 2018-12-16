using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Categories
{
    public class UpsertCategoriesCommandHandler : ICommandHandler<UpsertCategoriesCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertCategoriesCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertCategoriesCommand command)
        {
            _db.UpsertBulk(command.Categories);
        }
    }
}