using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Stocks
{
    public class UpsertStocksCommand : ICommand
    {
        public Stock[] Stocks { get; }

        public UpsertStocksCommand(Stock[] stocks)
        {
            Stocks = stocks;
        }
    }
}