using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Logic.StocksManagement;

namespace Logic
{
    public class StockProvider
    {
        public static ObservableCollection<Stock> Stocks { get; } = new ObservableCollection<Stock>();

        public static Stock GetStock(Guid id)
        {
            foreach (Stock s in Stocks)
            {
                if (s.Id.Equals(id))
                {
                    return s;
                }
            }
            
            Stock stock = new Stock("Loaded" + id, 0, id);
            Stocks.Add(stock);

            return stock;
        }

        public static void Add(Stock stock)
        {
            Stocks.Add(stock);
        }

        public static List<Stock> GetStocks()
        {
            if (Stocks.Count == 0 || !Stocks.Contains(Stock.Unknown))
            {
                Add(Stock.Unknown);
            }
            return new List<Stock>(Stocks);
        }
    }
}