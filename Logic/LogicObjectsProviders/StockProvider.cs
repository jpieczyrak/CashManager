using System;
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
        private static TrulyObservableCollection<Stock> _stocks;

        public static TrulyObservableCollection<Stock> Stocks => _stocks ?? (_stocks = Load());

        public static Stock GetStock(Guid id)
        {
            return Stocks.FirstOrDefault(s => s.Id.Equals(id));
        }

        public static void AddNew(string name = "", float value = 0f)
        {
            var stock = new Stock(name, value);
            Stocks.Add(stock);
            DatabaseProvider.DB.Update(Mapper.Map<Stock, DTO.Stock>(stock));
        }

        public static List<Stock> GetStocks()
        {
            if (Stocks.Count == 0 || !Stocks.Contains(Stock.Unknown))
            {
                Stocks.Add(Stock.Unknown);
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