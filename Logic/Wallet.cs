﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Logic.FilesOperations;
using Logic.LogicObjectsProviders;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic
{
    [DataContract(Namespace = "")]
    public class Wallet
    {
        public static string Path = "wallet.xml";

        [DataMember]
        public ObservableCollection<Stock> AvailableStocks
        {
            get { return StockProvider.Stocks; }
            set { StockProvider.Load(value); }
        }

        [DataMember]
        public Transactions Transactions { get; set; } = new Transactions();

        public void Save()
        {
            Serializer.XMLSerializeObjectToFile(this, Path);
            Transactions.Save(new CSVFormater(), string.Format("{0}-transactions.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
            //stocks & transactions
            //DBProvider.DB.Save();
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