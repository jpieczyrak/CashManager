using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Transactions
{
    public class UpsertTransactionsCommand : ICommand
    {
        public Transaction[] Transactions { get; }

        public UpsertTransactionsCommand(Transaction transaction)
        {
            Transactions = transaction != null ? new[] { transaction } : new Transaction[0];
        }

        public UpsertTransactionsCommand(Transaction[] transactions)
        {
            Transactions = transactions;
        }
    }
}