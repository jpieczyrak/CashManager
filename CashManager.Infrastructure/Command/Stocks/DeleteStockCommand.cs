using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Stocks
{
    public class DeleteStockCommand : ICommand
    {
        public Stock Stock { get; }

        public DeleteStockCommand(Stock stock)
        {
            Stock = stock;
        }
    }
}