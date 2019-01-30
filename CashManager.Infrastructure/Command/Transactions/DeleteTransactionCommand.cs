using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Transactions
{
    public class DeleteTransactionCommand : ICommand
    {
        public Transaction Transaction { get; }

        public DeleteTransactionCommand(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}