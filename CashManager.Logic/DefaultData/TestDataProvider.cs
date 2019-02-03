using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public class TestDataProvider : DefaultDataProvider
    {
        private readonly bool _moreData;
        private int _daysCount = 180;
        private int _titleCounter;
        private int _positionCounter;
        private readonly Random _random;
        private readonly decimal[] _vats = { 5m, 8m, 23m };

        private readonly Lazy<Category[]> _categories;
        private Stock _otherStock;

        public TestDataProvider(bool moreData = false)
        {
            _moreData = moreData;
            _random = new Random(1233213);
            GetCategories();
            GetTransactionTypes();
            GetTags();
            _categories = new Lazy<Category[]>(GetCategories);
        }

        public override Stock[] GetStocks()
        {
            _otherStock = new Stock { Name = "2nd", Balance = new Balance(DateTime.Today, 21555), IsUserStock = true };
            var stocks = new[]
            {
                new Stock { Name = Strings.DefaultUserBankAccount, IsUserStock = true, Balance = new Balance(DateTime.Today, 14543) },
                new Stock { Name = Strings.DefaultWallet, IsUserStock = true, Balance = new Balance(DateTime.Today, 10476) },
                new Stock { Name = "Ex1" },
                new Stock { Name = "Ex2" }
            };
            return _moreData ? stocks.Concat(new Stock[] { _otherStock }).ToArray() : stocks;
        }

        public override Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags)
        {
            if (_moreData) _daysCount *= 6;
            var userStock = stocks.FirstOrDefault(x => x.IsUserStock && x.Name == Strings.DefaultUserBankAccount);
            var externalStock = stocks.FirstOrDefault(x => !x.IsUserStock);

            var dtoTransactions = new[]
            {
                CreateTransaction(2, FindCategory("gry"), _buyType, userStock, externalStock, "some stuff", "fun"),
                CreateTransaction(12, FindCategory("jedzenie"), _buyType, userStock, externalStock, "food"),
                CreateTransaction(2, FindCategory("gry"), _buyType, userStock, externalStock, "games!!!"),
                CreateTransaction(6, FindCategory("gry"), _buyType, userStock, externalStock, "new collection", "mike told me to buy"),
                CreateTransaction(2, FindCategory("herbata"), _buyType, userStock, externalStock, "some tea for home"),
                CreateTransaction(1, FindCategory("Inne"), _giftsType, userStock, externalStock, "gift from mother"),
                CreateTransaction(2, FindCategory("Inne"), _workType, userStock, externalStock, "working"),
                CreateTransaction(1, FindCategory("Inne"), _workType, userStock, externalStock, "working", "it was profitable", 6),
                CreateTransaction(1, FindCategory("Inne"), _workType, userStock, externalStock, "working"),
                CreateTransaction(1, FindCategory("Inne"), _workType, userStock, externalStock, "working"),
                CreateTransaction(5, FindCategory("Inne"), _workType, userStock, externalStock, "working hard", "many tasks", 3),

                new Transaction(_workType, DateTime.Now.AddDays(-120), "work", "notes", new List<Position>
                    {
                        new Position
                        {
                            Category = FindCategory("Inne"),
                            Value = new PaymentValue { TaxPercentValue = 23, GrossValue = 12000 },
                            Title = "income",
                            LastEditDate = RandomDate()
                        }
                    }, stocks[0], stocks[3], "inputsource2"),

                new Transaction(_buyType, DateTime.Now.AddDays(-3).AddHours(12), "buying more stuff", "stuff!!!", new List<Position>
                    {
                        new Position
                        {
                            Category = FindCategory("książki"),
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
                            Category = FindCategory("rozrywka"),
                            Value = new PaymentValue { TaxPercentValue = 23, NetValue = 7129 },
                            Title = "new PC",
                            Tags = new List<Tag> { tags[1], tags[2] },
                            LastEditDate = RandomDate()
                        }
                    },
                    stocks[1], stocks[2], "inputsource4")
            };

            var transactions = dtoTransactions.Concat(
                                                  Enumerable.Range(0, _daysCount/4).Select(x =>
                                                      CreateTransaction(2, RandomCategory(), _buyType, userStock, externalStock, "unknown buys")))
                                              .Concat(
                                                  Enumerable.Range(0, _daysCount/30).Select(x =>
                                                      CreateTransaction(3, RandomCategory(), _workType, userStock, externalStock, "regular work", "work work", 4.44, DateTime.Today.AddMonths(-x))));

            if (_moreData)
            {
                transactions = transactions.Concat(
                                                  Enumerable.Range(0, 2222).Select(x =>
                                                      CreateTransaction(3, RandomCategory(), _buyType, _otherStock, externalStock, "unknown buys", "", 0.32)))
                                              .Concat(
                                                  Enumerable.Range(0, _daysCount / 10).Select(x =>
                                                      CreateTransaction(4, RandomCategory(), _workType, _otherStock, externalStock, "super work", "work x4", 4.77, DateTime.Today.AddDays(-(10 * x)))));
            }


            return transactions.ToArray();
        }

        private Transaction CreateTransaction(int positionsCount, Category category, TransactionType type, Stock userStock,
            Stock externalStock, string title = "", string note = "", double valueMultiplier = 1, DateTime? date = null)
        {
            var positions = Enumerable.Range(0, positionsCount).Select(x => new Position()).ToArray();
            foreach (var position in positions)
            {
                position.Title = $"position title {_positionCounter++}";
                double gross = _random.Next(25, 1250) * valueMultiplier;
                position.Value.GrossValue = (int)Math.Round(gross);
                position.Value.TaxPercentValue = _vats[_random.Next(0, _vats.Length)];
                position.Tags = new List<Tag>(_tags
                                              .OrderBy(x => _random.Next(0, 10))
                                              .Take(_random.Next(0, _tags.Length))
                                              .ToArray());
                position.Category = category;
                position.LastEditDate = date ?? RandomDate();
            }

            return new Transaction(type,
                date ?? RandomDate(),
                $"title {_titleCounter} {title}",
                $"note {_titleCounter} {note}",
                positions,
                userStock,
                externalStock,
                $"input source {_titleCounter++}");
        }

        private Category FindCategory(string name)
            => _categories.Value.FirstOrDefault(x => x.Name != null && x.Name.ToLower().Contains(name.ToLower()));

        private Category RandomCategory() => _categories.Value.OrderBy(x => _random.Next(10)).First();

        private DateTime RandomDate() => DateTime.Now.AddDays(-_random.Next(1, _daysCount));
    }
}