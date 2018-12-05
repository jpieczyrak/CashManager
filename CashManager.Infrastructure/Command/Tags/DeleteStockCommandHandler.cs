using CashManager.Infrastructure.DbConnection;

using LiteDB;

namespace CashManager.Infrastructure.Command.Tags
{
    public class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand>
    {
        private readonly LiteDatabase _db;

        public DeleteTagCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(DeleteTagCommand command)
        {
            _db.Remove(command.Tag);
        }
    }
}