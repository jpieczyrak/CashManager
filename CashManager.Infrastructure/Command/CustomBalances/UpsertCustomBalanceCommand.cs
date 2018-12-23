using CashManager.Data.ViewModelState.Balances;

namespace CashManager.Infrastructure.Command.CustomBalances
{
    public class UpsertCustomBalanceCommand : ICommand
    {
        public CustomBalance[] CustomBalances { get; }

        public UpsertCustomBalanceCommand(CustomBalance balance)
        {
            CustomBalances = new [] { balance };
        }

        public UpsertCustomBalanceCommand(CustomBalance[] balances)
        {
            CustomBalances = balances;
        }
    }
}