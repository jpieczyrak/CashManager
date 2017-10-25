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
        private static Stock _default;

        public static TrulyObservableCollection<Stock> Stocks => _stocks ?? (_stocks = Load());

        public static Stock GetStock(Guid id)
        {
            var stock = Stocks.FirstOrDefault(s => s.Id.Equals(id));
            return stock ?? Default;
        }

        private static readonly Guid _defaultGuid = new Guid(new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8});

        public static Stock Default { get; } = _default ?? (_default = Stocks.FirstOrDefault(x => x.Id == _defaultGuid) ?? AddNew(_defaultGuid, "Unknown"));

        public static Stock AddNew(string name = "", float value = 0f)
        {
            return AddNew(Guid.NewGuid(), name, value);
        }

        private static Stock AddNew(Guid guid, string name = "", float value = 0f)
        {
            var stock = new Stock(guid, name, value);
            Stocks.Add(stock);
            DatabaseProvider.DB.Update(Mapper.Map<Stock, DTO.Stock>(stock));

            return stock;
        }

        public static List<Stock> GetStocks()
        {
            if (Stocks.Count == 0 || Stocks.All(x => x.Id != Default.Id))
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