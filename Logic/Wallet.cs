using Logic.Database;
using Logic.LogicObjectsProviders;
using Logic.Model;
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
            foreach (var transaction in Transactions) DatabaseProvider.DB.Upsert(AutoMapper.Mapper.Map<Transaction, DTO.Transaction>(transaction));
            foreach (var stock in AvailableStocks) DatabaseProvider.DB.Upsert(AutoMapper.Mapper.Map<Stock, DTO.Stock>(stock));
            foreach (var category in Categories) DatabaseProvider.DB.Upsert(AutoMapper.Mapper.Map<Category, DTO.Category>(category));
        }
    }
}