using CashManager.WPF.Model;

namespace CashManager.WPF.Messages.Models
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