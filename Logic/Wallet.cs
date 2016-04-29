using System.Collections.Generic;
using System.Linq;
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
            //TODO: check if stock allready exists? (or check higher)
            Stocks.Add(stock);
        }

        public Stock GetStockByName(string stockName)
        {
            return Stocks.FirstOrDefault(stock => stock.ToString().ToLower().Equals(stockName.ToLower()));
        }
    }
}
