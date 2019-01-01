using CashManager_MVVM.Model;

namespace CashManager_MVVM.Messages
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