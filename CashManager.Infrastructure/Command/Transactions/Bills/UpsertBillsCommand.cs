using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Transactions.Bills
{
    public class UpsertBillsCommand : ICommand
    {
        public StoredFileInfo[] Bills { get; }

        public UpsertBillsCommand(StoredFileInfo bills)
        {
            Bills = bills != null ? new[] { bills } : new StoredFileInfo[0];
        }

        public UpsertBillsCommand(StoredFileInfo[] bills)
        {
            Bills = bills;
        }
    }
}