using System;

using LiteDB;

namespace CashManager.Infrastructure.Command.Transactions.Bills
{
    public class UpsertBillsCommandHandler : ICommandHandler<UpsertBillsCommand>
    {
        private readonly LiteDatabase _db;

        public UpsertBillsCommandHandler(LiteRepository repository)
        {
            _db = repository.Database;
        }

        public void Execute(UpsertBillsCommand command)
        {
            foreach (var bill in command.Bills)
            {
                _db.FileStorage.Upload(bill.DbAlias, bill.SourceName);
                var info = _db.FileStorage.FindById(bill.DbAlias);
                Console.WriteLine(info.Length);
            }
        }
    }
}