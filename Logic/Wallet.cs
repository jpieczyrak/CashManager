using System.Collections.ObjectModel;

using Logic.Database;
using Logic.LogicObjectsProviders;
using Logic.Model;
using Logic.StocksManagement;
using Logic.Utils;

namespace Logic
{
    public class Wallet
    {
        public TrulyObservableCollection<Stock> AvailableStocks => StockProvider.Stocks;

        public TrulyObservableCollection<Transaction> Transactions => TransactionProvider.Transactions;

        public TrulyObservableCollection<Category> Categories => CategoryProvider.Categories;

        public void Save()
        {
            //stocks & transactions
            foreach (var transaction in Transactions) DatabaseProvider.DB.Update(AutoMapper.Mapper.Map<Transaction, DTO.Transaction>(transaction));
            foreach (var stock in AvailableStocks) DatabaseProvider.DB.Update(AutoMapper.Mapper.Map<Stock, DTO.Stock>(stock));
            foreach (var category in Categories) DatabaseProvider.DB.Update(AutoMapper.Mapper.Map<Category, DTO.Category>(category));
        }

        public void UpdateStockStats(ObservableCollection<StockStats> stockStats, TimeFrame timeframe)
        {
            stockStats.Clear();
            foreach (var stock in AvailableStocks)
            {
                stockStats.Add(new StockStats(stock.Name, stock.GetActualValue(Transactions, timeframe)));
            }
        }
    }
}