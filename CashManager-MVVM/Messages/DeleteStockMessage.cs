using CashManager_MVVM.Model;

namespace CashManager_MVVM.Messages
{
    public class DeleteStockMessage
    {
        public Stock[] DeletedStocks { get; }

        public DeleteStockMessage(Stock deleted)
        {
            DeletedStocks = new[] { deleted };
        }

        public DeleteStockMessage(Stock[] deleted)
        {
            DeletedStocks = deleted;
        }
    }
}