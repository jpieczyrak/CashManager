using CashManager.Data.ViewModelState.Balances;

namespace CashManager.Infrastructure.Command.CustomBalances
{
    public class DeleteCustomBalanceCommand : ICommand
    {
        public CustomBalance CustomBalance { get; }

        public DeleteCustomBalanceCommand(CustomBalance balance)
        {
            CustomBalance = balance;
        }
    }
}