using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Data;
using CashManager.Data.DTO;

using DtoPaymentValue = CashManager.Data.DTO.PaymentValue;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Model.DataProviders
{
    public class DataService : IDataService
    {
        public void GetTransactions(Action<IEnumerable<Transaction>, Exception> callback)
        {
            IEnumerable<Transaction> transactions = null;// = TransactionProvider.Transactions;
#if DEBUG

            if (transactions == null || !transactions.Any())
            {
                var cats = new Category[] {};
                GetCategories((categories, exception) => cats = categories.ToArray());
                var trans = new List<DtoTransaction>
                {
                    new DtoTransaction(eTransactionType.Buy, DateTime.Now, "title1", "notes1", new List<CashManager.Data.DTO.Position>
                        {
                            new CashManager.Data.DTO.Position
                            {
                                Category = Mapper.Map<DtoCategory>(cats.FirstOrDefault(x => x.Parent == null)),
                                Value = new DtoPaymentValue { Value = 12 },
                                Title = "title cat1"
                            },
                            new CashManager.Data.DTO.Position
                            {
                                Category = Mapper.Map<DtoCategory>(cats.FirstOrDefault(x => x.Parent != null)),
                                Value = new DtoPaymentValue { Value = 15 },
                                Title = "title cat2"
                            },
                        }, 
                        new DtoStock
                        {
                            Name = "test1"
                        },
                        new DtoStock
                        {
                            Name = "test2"
                        }, "test1"),
                    new DtoTransaction(eTransactionType.Buy, DateTime.Now, "title2", "notes2", new List<CashManager.Data.DTO.Position>
                        {
                            new CashManager.Data.DTO.Position
                            {
                                Category = Mapper.Map<DtoCategory>(cats.FirstOrDefault(x => x.Parent == null)),
                                Value = new DtoPaymentValue { Value = 24 },
                                Title = "cat2"
                            }
                        }, 
                        new DtoStock
                        {
                            Name = "test1"
                        },
                        new DtoStock
                        {
                            Name = "test2"
                        }, "test2"),
                };
                transactions = trans.Select(Mapper.Map<Transaction>).ToArray();
            }
#endif
            callback(transactions, null);
        }

        public void GetCategories(Action<IEnumerable<Category>, Exception> callback)
        {
            IEnumerable<Category> categories = null;
#if DEBUG
            if (categories == null || !categories.Any())
            {
                var root = new DtoCategory { Id = Guid.NewGuid(), Value = "Root" };
                var home = new DtoCategory { Id = Guid.NewGuid(), Value = "Home", Parent = root };
                var fun = new DtoCategory { Id = Guid.NewGuid(), Value = "Fun", Parent = root };
                var fun_PC = new DtoCategory { Id = Guid.NewGuid(), Value = "PC", Parent = fun };
                var fun_books = new DtoCategory { Id = Guid.NewGuid(), Value = "Books", Parent = fun };
                var fun_games = new DtoCategory { Id = Guid.NewGuid(), Value = "Games", Parent = fun };
                var fun_games_strategy = new DtoCategory { Id = Guid.NewGuid(), Value = "Strategy", Parent = fun_games };
                var fun_games_fps = new DtoCategory { Id = Guid.NewGuid(), Value = "FPS", Parent = fun_games };
                var home_cleaning = new DtoCategory { Id = Guid.NewGuid(), Value = "Cleaning", Parent = home };
                var home_food = new DtoCategory { Id = Guid.NewGuid(), Value = "Food", Parent = home };
                var home_food_base = new DtoCategory { Id = Guid.NewGuid(), Value = "Base food", Parent = home_food };
                var home_food_chocolates = new DtoCategory { Id = Guid.NewGuid(), Value = "Chocolates", Parent = home_food };
                var home_food_tea = new DtoCategory { Id = Guid.NewGuid(), Value = "Tea", Parent = home_food };
                var dtoCategories = new List<DtoCategory>
                {
                    root,
                    home, fun,
                    fun_PC, fun_books, fun_games,
                    fun_games_strategy, fun_games_fps, 
                    home_cleaning, home_food,
                    home_food_base, home_food_chocolates, home_food_tea
                };
                categories = dtoCategories.Select(Mapper.Map<Category>).ToArray();
            }
#endif
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
    }
}