using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public class DefaultDataProvider : IDataProvider
    {
        private Category _root;
        private Category _home;
        private Category _fun;
        private Category _fun_pc;
        private Category _fun_books;
        private Category _fun_games;
        private Category _fun_games_strategy;
        private Category _fun_games_fps;
        private Category _home_cleaning;
        private Category _home_food;
        private Category _home_food_base;
        private Category _home_food_chocolates;
        private Category _home_food_tea;
        private TransactionType _workType;
        private TransactionType _buyType;
        private TransactionType _transferInType;
        private TransactionType _transferOutType;
        private TransactionType _giftsType;
        private int _titleCounter = 0;
        private int _positionCounter = 0;
        private readonly Random _random;
        private readonly decimal[] _vats = new[] { 5m, 8m, 23m };
        private Tag[] _tags;

        public DefaultDataProvider()
        {
            _random = new Random(1233213);
            GetCategories();
            GetTransactionTypes();
            GetTags();
        }

        public Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags)
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
                CreateTransaction(1, _root, _giftsType, userStock, externalStock, "gift from mother"),
                CreateTransaction(1, _root, _workType, userStock, externalStock, "working"),
                CreateTransaction(1, _root, _workType, userStock, externalStock, "working", "it was profitable", 10),
                CreateTransaction(1, _root, _workType, userStock, externalStock, "working"),
                CreateTransaction(1, _root, _workType, userStock, externalStock, "working"),
                CreateTransaction(5, _root, _workType, userStock, externalStock, "working hard", "many tasks", 3),
                new Transaction(_workType, DateTime.Now.AddDays(-90), "work", "notes", new List<Position>
                    {
                        new Position
                        {
                            Category = _root,
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
                        },
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
                        },
                    },
                    stocks[1], stocks[2], "inputsource4"),
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

        public Category[] GetCategories()
        {
            _root = new Category { Name = "Root" };
            _home = new Category { Name = "Home", Parent = _root };
            _fun = new Category { Name = "Fun", Parent = _root };
            _fun_pc = new Category { Name = "PC", Parent = _fun };
            _fun_books = new Category { Name = "Books", Parent = _fun };
            _fun_games = new Category { Name = "Games", Parent = _fun };
            _fun_games_strategy = new Category { Name = "Strategy", Parent = _fun_games };
            _fun_games_fps = new Category { Name = "FPS", Parent = _fun_games };
            _home_cleaning = new Category { Name = "Cleaning", Parent = _home };
            _home_food = new Category { Name = "Food", Parent = _home };
            _home_food_base = new Category { Name = "Base food", Parent = _home_food };
            _home_food_chocolates = new Category { Name = "Chocolates", Parent = _home_food };
            _home_food_tea = new Category { Name = "Tea", Parent = _home_food };
            var dtoCategories = new[]
            {
                _root,
                _home,
                _fun,
                _fun_pc,
                _fun_books,
                _fun_games,
                _fun_games_strategy,
                _fun_games_fps,
                _home_cleaning,
                _home_food,
                _home_food_base,
                _home_food_chocolates,
                _home_food_tea
            };

            return dtoCategories;
        }

        public Stock[] GetStocks()
        {
            return new[]
            {
                new Stock { Name = "User1", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 60000) },
                new Stock { Name = "Wallet", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 10476) },
                new Stock { Name = "Ex1" },
                new Stock { Name = "Ex2" }
            };
        }

        public TransactionType[] GetTransactionTypes()
        {
            _workType = new TransactionType { Income = true, Name = "Work", IsDefault = true };
            _buyType = new TransactionType { Outcome = true, Name = "Buy", IsDefault = true };
            _transferInType = new TransactionType { Name = "Transfer in", Income = true };
            _transferOutType = new TransactionType { Name = "Transfer out", Outcome = true };
            _giftsType = new TransactionType { Income = true, Name = "Gifts" };
            return new[]
            {
                _workType,
                _buyType,
                _transferInType,
                _transferOutType,
                _giftsType
            };
        }

        public Tag[] GetTags()
        {
            _tags = new Tag[]
            {
                new Tag { Name = "tag 1" },
                new Tag { Name = "tag 2" },
                new Tag { Name = "tag 3" },
            };
            return _tags;
        }
    }
}