using System.Collections.Generic;
using Logic.StocksManagement;

namespace Logic
{
    public class Wallet
    {
        public List<Stock> Stocks { get; } = new List<Stock>();

        public Wallet()
        {
        }

        public void AddStock(Stock stock)
        {
            Stocks.Add(stock);
        }
    }
}
