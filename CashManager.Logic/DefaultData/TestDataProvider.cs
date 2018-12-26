using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public class TestDataProvider : DefaultDataProvider
    {
        private int _titleCounter;
        private int _positionCounter;
        private readonly Random _random;
        private readonly decimal[] _vats = { 5m, 8m, 23m };

        public TestDataProvider()
        {
            _random = new Random(1233213);
            GetCategories();
            GetTransactionTypes();
            GetTags();
        }

        public override Stock[] GetStocks()
        {
            return new[]
            {
                new Stock { Name = "User1", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 60000) },
                new Stock { Name = "Wallet", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 10476) },
                new Stock { Name = "Ex1" },
                new Stock { Name = "Ex2" }
            };
        }

        public override Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags)
        {
            var userStock = stocks.FirstOrDefault(x => x.IsUserStock);
            var externalStock = stocks.FirstOrDefault(x => !x.IsUserStock);

            var dtoTransactions = new[]
            {
                CreateTransaction(2, _fun, _buyType, userStock, externalStock, "some stuff", "fun"),
                CreateTransaction(3, _home_food_base, _buyType, userStock, externalStock, "food"),
                CreateTransaction(1, _fun_games_strategy, _buyType, userStock, externalStock, "games!!!"),
                CreateTransaction(10, _fun_games, _buyType, userStock, externalStock, "new collection", "mike told me to buy"),
                CreateTransaction(2, _home_food_tea, _buyType, userStock, externalStock, "some tea for home"),
                CreateTransaction(1, _unknown, _giftsType, userStock, externalStock, "gift from mother"),
                CreateTransaction(1, _unknown, _workType, userStock, externalStock, "working"),
                CreateTransaction(1, _unknown, _workType, userStock, externalStock, "working", "it was profitable", 10),
                CreateTransaction(1, _unknown, _workType, userStock, externalStock, "working"),
                CreateTransaction(1, _unknown, _workType, userStock, externalStock, "working"),
                CreateTransaction(5, _unknown, _workType, userStock, externalStock, "working hard", "many tasks", 3),
                new Transaction(_workType, DateTime.Now.AddDays(-90), "work", "notes", new List<Position>
                    {
                        new Position
                        {
                            Category = _unknown,
                            Value = new PaymentValue { TaxPercentValue = 23, GrossValue = 25000 },
                            Title = "income",
                            LastEditDate = RandomDate()
                        }
                    }, stocks[0], stocks[3], "inputsource2"),

                new Transaction(_buyType, DateTime.Now.AddDays(-3).AddHours(12), "buying more stuff", "stuff!!!", new List<Position>
                    {
                        new Position
                        {
                            Category = _fun_books,
                            Value = new PaymentValue { TaxPercentValue = 23, GrossValue = 2499 },
                            Title = "sth expensive",
                            Tags = new List<Tag> { tags[0] },
                            LastEditDate = RandomDate()
                        }
                    },
                    stocks[1], stocks[3], "inputsource3"),
                new Transaction(_buyType, DateTime.Now.AddDays(-8), "it is time for PC", "best pc ever", new List<Position>
                    {
                        new Position
                        {
                            Category = _fun_pc,
                            Value = new PaymentValue { TaxPercentValue = 23, NetValue = 7129 },
                            Title = "new PC",
                            Tags = new List<Tag> { tags[1], tags[2] },
                            LastEditDate = RandomDate()
                        }
                    },
                    stocks[1], stocks[2], "inputsource4")
            };

            return dtoTransactions;
        }

        private DateTime RandomDate() => DateTime.Now.AddDays(-_random.Next(1, 60));

        private Transaction CreateTransaction(int positionsCount, Category category, TransactionType type, Stock userStock,
            Stock externalStock,
            string title = "", string note = "", int valueMultiplier = 1)
        {
            var positions = Enumerable.Range(0, positionsCount).Select(x => new Position()).ToArray();
            foreach (var position in positions)
            {
                position.Title = $"position title {_positionCounter++}";
                decimal gross = _random.Next(25, 1250) * valueMultiplier;
                position.Value.GrossValue = gross;
                position.Value.TaxPercentValue = _vats[_random.Next(0, _vats.Length)];
                position.Tags = new List<Tag>(_tags
                                              .OrderBy(x => _random.Next(0, 10))
                                              .Take(_random.Next(0, _tags.Length))
                                              .ToArray());
                position.Category = category;
                position.LastEditDate = RandomDate();
            }

            return new Transaction(type, 
                RandomDate(), 
                $"title {_titleCounter} {title}", 
                $"note {_titleCounter} {note}",
                positions,
                userStock, 
                externalStock, 
                $"input source {_titleCounter++}");
        }
    }
}