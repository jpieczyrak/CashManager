using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

using Logic.Database;
using Logic.FilesOperations;
using Logic.LogicObjectsProviders;
using Logic.Model;
using Logic.StocksManagement;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

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
            foreach (var transaction in Transactions.TransactionsList)
            {
                DatabaseProvider.DB.Update(AutoMapper.Mapper.Map<Transaction, DTO.Transaction>(transaction));
            }
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