using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.TransactionTypes
{
    public class UpsertTransactionTypesCommand : ICommand
    {
        public TransactionType[] Types { get; }

        public UpsertTransactionTypesCommand(TransactionType type)
        {
            Types = type != null ? new[] { type } : new TransactionType[0];
        }

        public UpsertTransactionTypesCommand(TransactionType[] types)
        {
            Types = types;
        }
    }
}