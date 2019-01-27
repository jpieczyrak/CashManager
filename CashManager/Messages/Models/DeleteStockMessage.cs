using CashManager.Model;

namespace CashManager.Messages.Models
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