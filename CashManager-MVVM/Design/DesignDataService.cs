using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Data;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.DataProviders;

using PaymentValue = CashManager.Data.DTO.PaymentValue;
using Subtransaction = CashManager.Data.DTO.Subtransaction;
using Tag = CashManager.Data.DTO.Tag;

namespace CashManager_MVVM.Design
{
    public class DesignDataService : IDataService
    {
        #region IDataService

        public void GetTransactions(Action<IEnumerable<Transaction>, Exception> callback)
        {
            var cats = new Category[] { };
            GetCategories((categories, exception) => cats = categories.ToArray());
            var transactions = new List<CashManager.Data.DTO.Transaction>
            {
                new CashManager.Data.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title1 - design", "notes1", new List<Subtransaction>
                    {
                        new Subtransaction
                        {
                            Category = Mapper.Map<CashManager.Data.DTO.Category>(cats.FirstOrDefault(x => x.Parent == null)),
                            Value = new PaymentValue { Value = 12 },
                            Title = "cat1",
                            Tags = new List<Tag> { new Tag { Name = "tag1" } }
                        }
                    }, new CashManager.Data.DTO.Stock
                    {
                        Name = "test1"
                    },
                    new CashManager.Data.DTO.Stock
                    {
                        Name = "test2"
                    }, "design1"),
                new CashManager.Data.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title2", "notes2", new List<Subtransaction>
                    {
                        new Subtransaction
                        {
                            Category = Mapper.Map<CashManager.Data.DTO.Category>(cats.FirstOrDefault(x => x.Parent != null)),
                            Value = new PaymentValue { Value = 24 },
                            Title = "cat2",
                            Tags = new List<Tag> { new Tag { Name = "tag123" } }
                        }
                    }, new CashManager.Data.DTO.Stock
                    {
                        Name = "test1"
                    },
                    new CashManager.Data.DTO.Stock
                    {
                        Name = "test2"
                    }, "design2")
            };
            callback(transactions.Select(Mapper.Map<Transaction>), null);
        }

        public void GetCategories(Action<IEnumerable<Category>, Exception> callback)
        {
            var root = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Root" };
            var home = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Home", Parent = root };
            var fun = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Fun", Parent = root };
            var fun_PC = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "PC", Parent = fun };
            var fun_books = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Books", Parent = fun };
            var fun_games = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Games", Parent = fun };
            var fun_games_strategy = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Strategy", Parent = fun_games };
            var fun_games_fps = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "FPS", Parent = fun_games };
            var home_cleaning = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Cleaning", Parent = home };
            var home_food = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Food", Parent = home };
            var home_food_base = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Base food", Parent = home_food };
            var home_food_chocolates = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Chocolates", Parent = home_food };
            var home_food_tea = new CashManager.Data.DTO.Category { Id = Guid.NewGuid(), Value = "Tea", Parent = home_food };
            var dtoCategories = new List<CashManager.Data.DTO.Category>
            {
                root,
                home, fun,
                fun_PC, fun_books, fun_games,
                fun_games_strategy, fun_games_fps,
                home_cleaning, home_food,
                home_food_base, home_food_chocolates, home_food_tea
            };
            var categories = dtoCategories.Select(Mapper.Map<Category>).ToArray();
            callback(categories, null);
        }

        public void GetStocks(Action<IEnumerable<Stock>, Exception> callback)
        {
            callback(
                new List<Stock>
                {
                    new Stock { Name = "User1", IsUserStock = true },
                    new Stock { Name = "Ex1" },
                    new Stock { Name = "Ex2" }
                }, null);
        }

        #endregion
    }
}