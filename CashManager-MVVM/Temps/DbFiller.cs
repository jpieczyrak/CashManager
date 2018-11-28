using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using AutoMapper;

using CashManager.Data;
using CashManager.Data.DTO;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Transactions;

using PaymentValue = CashManager_MVVM.Model.PaymentValue;
using Category = CashManager_MVVM.Model.Category;
using Transaction = CashManager_MVVM.Model.Transaction;
using Stock = CashManager_MVVM.Model.Stock;

using DtoPaymentValue = CashManager.Data.DTO.PaymentValue;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Temps
{
    public class DbFiller
    {
        public static void Fill(IContainer container)
        {
#if DEBUG
            var queryDispatcher = container.Resolve<IQueryDispatcher>();
            var commandDispatcher = container.Resolve<ICommandDispatcher>();

            var stocks = queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery());
            if (stocks.Length < 2)
            {
                stocks = stocks.Concat(GetStocks()).Distinct().ToArray();
                //save stocks to db
            }

            if (!queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()).Any())
            {
                commandDispatcher.Execute(new UpsertTransactionsCommand(GetTransactions(stocks)));
            }
#endif
        }

        private static DtoTransaction[] GetTransactions(DtoStock[] stocks)
        {
            var categories = GetCategories();
            var dtoTransactions = new[]
            {
                new DtoTransaction(eTransactionType.Buy, DateTime.Now, "title1", "notes1", new List<Position>
                    {
                        new Position
                        {
                            Category = Mapper.Map<DtoCategory>(categories.FirstOrDefault(x => x.Parent == null)),
                            Value = new DtoPaymentValue { Value = 12 },
                            Title = "title cat1"
                        },
                        new Position
                        {
                            Category = Mapper.Map<DtoCategory>(categories.FirstOrDefault(x => x.Parent != null)),
                            Value = new DtoPaymentValue { Value = 15 },
                            Title = "title cat2"
                        }
                    },
                    stocks[0], stocks[1], "test1"),
                new DtoTransaction(eTransactionType.Buy, DateTime.Now, "title2", "notes2", new List<Position>
                    {
                        new Position
                        {
                            Category = Mapper.Map<DtoCategory>(categories.FirstOrDefault(x => x.Parent == null)),
                            Value = new DtoPaymentValue { Value = 24 },
                            Title = "cat2"
                        }
                    }, stocks[0], stocks[2], "test2")
            };

            return dtoTransactions;
        }

        private static DtoCategory[] GetCategories()
        {
            var root = new DtoCategory { Value = "Root" };
            var home = new DtoCategory { Value = "Home", Parent = root };
            var fun = new DtoCategory { Value = "Fun", Parent = root };
            var fun_PC = new DtoCategory { Value = "PC", Parent = fun };
            var fun_books = new DtoCategory { Value = "Books", Parent = fun };
            var fun_games = new DtoCategory { Value = "Games", Parent = fun };
            var fun_games_strategy = new DtoCategory { Value = "Strategy", Parent = fun_games };
            var fun_games_fps = new DtoCategory { Value = "FPS", Parent = fun_games };
            var home_cleaning = new DtoCategory { Value = "Cleaning", Parent = home };
            var home_food = new DtoCategory { Value = "Food", Parent = home };
            var home_food_base = new DtoCategory { Value = "Base food", Parent = home_food };
            var home_food_chocolates = new DtoCategory { Value = "Chocolates", Parent = home_food };
            var home_food_tea = new DtoCategory { Value = "Tea", Parent = home_food };
            var dtoCategories = new[]
            {
                root,
                home,
                fun,
                fun_PC,
                fun_books,
                fun_games,
                fun_games_strategy,
                fun_games_fps,
                home_cleaning,
                home_food,
                home_food_base,
                home_food_chocolates,
                home_food_tea
            };

            return dtoCategories;
        }

        private static DtoStock[] GetStocks()
        {
            return new[]
            {
                new DtoStock { Name = "User1", IsUserStock = true },
                new DtoStock { Name = "Ex1" },
                new DtoStock { Name = "Ex2" }
            };
        }
    }
}