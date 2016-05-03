using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Logic.StocksManagement;

namespace Logic
{
    [DataContract]
    public class Wallet
    {
        [DataMember]
        public List<Stock> AvailableStocks { get; } = new List<Stock>();

        public Wallet()
        {
            //Stocks.AvailableStocks = AvailableStocks;
        }

        public void AddStock(Stock stock)
        {
            //TODO: check if stock allready exists? (or check higher)
            AvailableStocks.Add(stock);
        }


        //TODO: to dict
        public Stock GetStockByName(string stockName)
        {
            return AvailableStocks.FirstOrDefault(stock => stock.ToString().ToLower().Equals(stockName.ToLower()));
        }
    }
}
