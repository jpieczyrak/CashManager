using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Transactions
{
    public class UpsertTransactionsCommand : ICommand
    {
        public UpsertTransactionsCommand(Transaction[] transactions)
        {
            Transactions = transactions;
        }

        public Transaction[] Transactions { get; }
    }
}