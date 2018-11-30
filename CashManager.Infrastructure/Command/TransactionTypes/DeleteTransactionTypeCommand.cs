using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.TransactionTypes
{
    public class DeleteTransactionTypeCommand : ICommand
    {
        public TransactionType TransactionType { get; }

        public DeleteTransactionTypeCommand(TransactionType transactionType)
        {
            TransactionType = transactionType;
        }
    }
}