using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Transactions
{
    public class UpsertTransactionsCommand : ICommand
    {
        public Transaction[] Transactions { get; }

        public UpsertTransactionsCommand(Transaction transaction)
        {
            Transactions = new Transaction[] { transaction };
        }

        public UpsertTransactionsCommand(Transaction[] transactions)
        {
            Transactions = transactions;
        }
    }
}