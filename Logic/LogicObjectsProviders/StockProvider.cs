using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using Logic.Database;
using Logic.Model;
using Logic.Utils;

namespace Logic.LogicObjectsProviders
{
    public class StockProvider
    {
        private const string DEFAULT_STOCK_NAME = "Unknown";
        private static TrulyObservableCollection<Stock> _stocks;
        private static Stock _default;

        public static TrulyObservableCollection<Stock> Stocks => _stocks ?? (_stocks = Load());

        public static Stock GetStock(string name)
        {
            var stock = Stocks.FirstOrDefault(s => s.Name.Equals(name));
            return stock ?? Default;
        }

        public static Stock Default { get; } = _default ?? (_default = Stocks.FirstOrDefault(x => x.Name == DEFAULT_STOCK_NAME) ?? AddNew(DEFAULT_STOCK_NAME));

        public static Stock AddNew(string name)
        {
            var stock = new Stock(name);
            Stocks.Add(stock);
            DatabaseProvider.DB.Update(Mapper.Map<Stock, DTO.Stock>(stock));

            return stock;
        }
        
        public static List<Stock> GetStocks()
        {
            if (Stocks.Count == 0 || !Stocks.Any(x => x.Name == DEFAULT_STOCK_NAME))
            {
                Stocks.Add(Default);
            }
            return new List<Stock>(Stocks);
        }

        public static TrulyObservableCollection<Stock> Load()
        {
            var dtos = DatabaseProvider.DB.Read<DTO.Stock>();
            var list = dtos.Select(Mapper.Map<DTO.Stock, Stock>);
            _stocks = new TrulyObservableCollection<Stock>(list);
            _stocks.CollectionChanged += StocksOnCollectionChanged;

            return _stocks;
        }

        private static void StocksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.NewItems != null)
            {
                foreach (Stock stock in notifyCollectionChangedEventArgs.NewItems)
                {
                    DatabaseProvider.DB.Update(Mapper.Map<Stock, DTO.Stock>(stock));
                }
            }
            if (notifyCollectionChangedEventArgs.OldItems != null)
            {
                foreach (Stock stock in notifyCollectionChangedEventArgs.OldItems)
                {
                    DatabaseProvider.DB.Remove(Mapper.Map<Stock, DTO.Stock>(stock));
                }
            }
        }
    }
}