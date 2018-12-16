using CashManager_MVVM.Model;

namespace CashManager_MVVM.Messages
{
    public class StockUpdateMessage
    {
        public Stock[] UpdatedStocks { get; private set; }

        public StockUpdateMessage(Stock[] updatedStocks)
        {
            UpdatedStocks = updatedStocks;
        }

        public StockUpdateMessage(Stock stock)
        {
            UpdatedStocks = new[] { stock };
        }
    }
}