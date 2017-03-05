using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AutoMapper;

using Logic.Database;
using Logic.Model;

namespace Logic.LogicObjectsProviders
{
    public class StockProvider
    {
        public static ObservableCollection<Stock> Stocks { get; } = new ObservableCollection<Stock>();

        public static Stock GetStock(Guid id)
        {
            return Stocks.FirstOrDefault(s => s.Id.Equals(id));
        }

        public static void AddNew(string name = "", float value = 0f)
        {
            var stock = new Stock(name, value);
            Stocks.Add(stock);
            DatabaseProvider.DB.Save(stock);
        }

        public static List<Stock> GetStocks()
        {
            if (Stocks.Count == 0 || !Stocks.Contains(Stock.Unknown))
            {
                Stocks.Add(Stock.Unknown);
            }
            return new List<Stock>(Stocks);
        }

        public static void Load()
        {
            var dtos = DatabaseProvider.DB.Read<DTO.Stock>();
            foreach (var dto in dtos)
            {
                Stocks.Add(Mapper.Map<DTO.Stock, Stock>(dto));
            }
        }
    }
}