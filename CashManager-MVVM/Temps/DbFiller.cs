﻿using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using CashManager.Data;
using CashManager.Data.DTO;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Command.Tags;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Command.TransactionTypes;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;

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

            if (!queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()).Any())
            {
                var stocks = GetStocks();
                var categories = GetCategories();
                var types = GetTransactionTypes();
                var transactions = GetTransactions(stocks, categories, types);
                var positions = transactions.SelectMany(x => x.Positions).ToArray();

                commandDispatcher.Execute(new UpsertStocksCommand(stocks));
                commandDispatcher.Execute(new UpsertTransactionTypesCommand(types));
                commandDispatcher.Execute(new UpsertCategoriesCommand(categories));
                commandDispatcher.Execute(new UpsertTagsCommand(positions.Where(x => x.Tags != null).SelectMany(x => x.Tags).ToArray()));
                commandDispatcher.Execute(new UpsertTransactionsCommand(transactions));
            }
#endif
        }

        private static Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types)
        {
            var dtoTransactions = new[]
            {
                new DtoTransaction(types[1], DateTime.Now, "title 1", "notes 1", new List<Position>
                    {
                        new Position
                        {
                            Category = categories.FirstOrDefault(x => x.Parent == null),
                            Value = new DtoPaymentValue { Value = 10 },
                            Title = "my position 1"
                        },
                        new Position
                        {
                            Category = categories.FirstOrDefault(x => x.Parent != null),
                            Value = new DtoPaymentValue { Value = 15 },
                            Title = "my position 2"
                        }
                    },
                    stocks[0], stocks[1], "inputsource1"),
                new DtoTransaction(types[0], DateTime.Now, "title 2 - work", "notes 2", new List<Position>
                    {
                        new Position
                        {
                            Category = categories.Skip(5).FirstOrDefault(x => x.Parent != null),
                            Value = new DtoPaymentValue { Value = 1000 },
                            Title = "income"
                        }
                    }, stocks[0], stocks[2], "inputsource2")
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

        private static TransactionType[] GetTransactionTypes()
        {
            return new[]
            {
                new TransactionType { Income = true, Name = "Work" },
                new TransactionType { Outcome = true, Name = "Buy" },
                new TransactionType { Name = "Transfer" }
            };
        }
    }
}