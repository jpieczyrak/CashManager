using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Categories
{
    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
    {
        private readonly LiteDatabase _db;

        public DeleteCategoryCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(DeleteCategoryCommand command)
        {
            _db.Remove(command.Category);
        }
    }
}