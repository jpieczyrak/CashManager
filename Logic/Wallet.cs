﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Logic.FilesOperations;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace Logic
{
    [DataContract(Namespace = "")]
    public class Wallet
    {
        public static string Path = "wallet.xml";
        private ObservableCollection<Stock> _availableStocks = new ObservableCollection<Stock>();

        [DataMember]
        public ObservableCollection<Stock> AvailableStocks
        {
            get
            {
                if (_availableStocks.Count == 0 || !_availableStocks.Contains(Stock.Unknown))
                {
                    _availableStocks.Add(Stock.Unknown);
                }
                return _availableStocks;
            }
            set { _availableStocks = value; }
        }

        [DataMember]
        public Transactions Transactions { get; set; } = new Transactions();

        public void Save()
        {
            Serializer.XMLSerializeObject(this, Path);
            Transactions.Save(new CSVFormater(), string.Format("{0}-transactions.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        public void UpdateStockStats(ObservableCollection<StockStats> stockStats, TimeFrame timeframe)
        {
            stockStats.Clear();
            foreach (Stock stock in AvailableStocks)
            {
                stockStats.Add(new StockStats(stock.Name, stock.GetActualValue(Transactions, timeframe)));
            }
        }
    }
}