using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public class DefaultDataProvider : IDataProvider
    {
        public Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags)
        {
            var dtoTransactions = new[]
            {
                new Transaction(types[1], DateTime.Now.AddDays(-45), "title 1 - buying some stuff", "notes 1", new List<Position>
                    {
                        new Position
                        {
                            Category = categories.FirstOrDefault(x => x.Parent == null),
                            Value = new PaymentValue { TaxPercentValue = 8, GrossValue = 10},
                            Title = "my position 1 - apples",
                            Tags = new List<Tag> { tags[0] }
                        },
                        new Position
                        {
                            Category = categories.FirstOrDefault(x => x.Parent != null),
                            Value = new PaymentValue {  TaxPercentValue = 5, GrossValue = 15 },
                            Title = "my position 2 - tea",
                            Tags = new List<Tag> { tags[1] }
                        }
                    },
                    stocks[0], stocks[2], "inputsource1"),
                new Transaction(types[0], DateTime.Now.AddDays(-30), "title 2 - work", "notes 2", new List<Position>
                    {
                        new Position
                        {
                            Category = categories.Skip(5).FirstOrDefault(x => x.Parent != null),
                            Value = new PaymentValue { TaxPercentValue = 23, NetValue = 1000 },
                            Title = "income",
                            Tags = new List<Tag> { tags[0], tags[2] }
                        }
                    }, stocks[0], stocks[3], "inputsource2"),
                new Transaction(types[1], DateTime.Now.AddDays(-20).AddHours(12), "title 3 - buying more stuff", "stuff!!!", new List<Position>
                    {
                        new Position
                        {
                            Category = categories.FirstOrDefault(x => x.Parent == null),
                            Value = new PaymentValue { TaxPercentValue = 23, GrossValue = 2499 },
                            Title = "sth expensive",
                            Tags = new List<Tag> { tags[0] }
                        },
                    },
                    stocks[1], stocks[3], "inputsource3"),
                new Transaction(types[1], DateTime.Now, "title 4 - buying even more stuff", "stuff, stuff!!!", new List<Position>
                    {
                        new Position
                        {
                            Category = categories.FirstOrDefault(x => x.Parent == null),
                            Value = new PaymentValue { TaxPercentValue = 23, NetValue = 7129 },
                            Title = "sth expensive 2",
                            Tags = new List<Tag> { tags[1], tags[2] }
                        },
                    },
                    stocks[1], stocks[2], "inputsource4"),
            };

            return dtoTransactions;
        }

        public Category[] GetCategories()
        {
            var root = new Category { Name = "Root" };
            var home = new Category { Name = "Home", Parent = root };
            var fun = new Category { Name = "Fun", Parent = root };
            var fun_PC = new Category { Name = "PC", Parent = fun };
            var fun_books = new Category { Name = "Books", Parent = fun };
            var fun_games = new Category { Name = "Games", Parent = fun };
            var fun_games_strategy = new Category { Name = "Strategy", Parent = fun_games };
            var fun_games_fps = new Category { Name = "FPS", Parent = fun_games };
            var home_cleaning = new Category { Name = "Cleaning", Parent = home };
            var home_food = new Category { Name = "Food", Parent = home };
            var home_food_base = new Category { Name = "Base food", Parent = home_food };
            var home_food_chocolates = new Category { Name = "Chocolates", Parent = home_food };
            var home_food_tea = new Category { Name = "Tea", Parent = home_food };
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

        public Stock[] GetStocks()
        {
            return new[]
            {
                new Stock { Name = "User1", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 100)},
                new Stock { Name = "Wallet", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 1048576) },
                new Stock { Name = "Ex1" },
                new Stock { Name = "Ex2" }
            };
        }

        public TransactionType[] GetTransactionTypes()
        {
            return new[]
            {
                new TransactionType { Income = true, Name = "Work", IsDefault = true },
                new TransactionType { Outcome = true, Name = "Buy", IsDefault = true },
                new TransactionType { Name = "Transfer" },
                new TransactionType { Income = true, Name = "Gifts" }
            };
        }

        public Tag[] GetTags()
        {
            return new Tag[]
            {
                new Tag { Name = "tag 1" },
                new Tag { Name = "tag 2" },
                new Tag { Name = "tag 3" },
            };
        }
    }
}