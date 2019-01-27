using CashManager.WPF.Model;

namespace CashManager.WPF.Messages.Models
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