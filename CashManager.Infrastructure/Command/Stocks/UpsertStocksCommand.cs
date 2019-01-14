using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Stocks
{
    public class UpsertStocksCommand : ICommand
    {
        public Stock[] Stocks { get; }

        public UpsertStocksCommand(Stock stock)
        {
            Stocks = new[] { stock };
        }

        public UpsertStocksCommand(Stock[] stocks)
        {
            Stocks = stocks;
        }
    }
}