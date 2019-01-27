using CashManager.Model;

namespace CashManager.Messages.Models
{
    public class UpdateStockMessage
    {
        public Stock[] UpdatedStocks { get; private set; }

        public UpdateStockMessage(Stock[] updatedStocks)
        {
            UpdatedStocks = updatedStocks;
        }

        public UpdateStockMessage(Stock stock)
        {
            UpdatedStocks = new[] { stock };
        }
    }
}