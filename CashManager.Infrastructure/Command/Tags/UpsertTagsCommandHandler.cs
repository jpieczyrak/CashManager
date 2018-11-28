using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Tags
{
    public class UpsertTagsCommandHandler : ICommandHandler<UpsertTagsCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertTagsCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertTagsCommand command)
        {
            _db.UpsertBulk(command.Tags);
        }
    }
}