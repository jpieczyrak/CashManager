using System;
using System.Linq;

using Bogus;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public class MassDataProvider : IDataProvider
    {
        public MassDataProvider()
        {
            Randomizer.Seed = new Random(123321);
        }

        #region IDataProvider

        public Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags)
        {
            int counter = 1;
            var possibleTaxes = new[] { 5m, 8m, 23m };
            var userStocks = stocks.Where(x => x.IsUserStock).ToArray();
            var externalStocks = stocks.Where(x => !x.IsUserStock).ToArray();
            var positionsFactory = new Faker<Position>()
                .RuleFor(position => position.Title, faker => faker.Commerce.Product())
                .RuleFor(position => position.Tags, faker => faker.PickRandom(tags, faker.Random.Int(0, 3)).ToList())
                .RuleFor(position => position.Value, faker =>
                                   {
                                       decimal netValue = faker.Random.Decimal(0m, 5000m);
                                       decimal tax = faker.PickRandom(possibleTaxes);
                                       return new PaymentValue
                                       {
                                           NetValue = netValue,
                                           TaxPercentValue = tax,
                                           GrossValue = netValue * (100m + tax) / 100m
                                       };
                                   })
                .RuleFor(position => position.Category, faker => faker.PickRandom(categories));

            var factory = new Faker<Transaction>()
                .CustomInstantiator(faker => new Transaction(
                    faker.PickRandom(types),
                    faker.Date.Recent(365),
                    faker.Commerce.Product(),
                    faker.Commerce.Ean8(),
                    positionsFactory.Generate(faker.Random.Int(1, 5)),
                    faker.PickRandom(userStocks),
                    faker.PickRandom(externalStocks),
                    $"input source {counter++}"));
            return factory.Generate(250).ToArray();
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
            var factory = new Faker<Stock>()
                .RuleFor(stock => stock.Name, (faker, stock) => faker.Company.CompanySuffix())
                .RuleFor(stock => stock.IsUserStock, (faker, stock) => faker.Random.Bool())
                .RuleFor(stock => stock.Balance, (faker, stock) => new Balance(faker.Date.Recent(30), faker.Random.Decimal(10m, 1000000m)));
            return factory.Generate(5).ToArray();
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
            var factory = new Faker<Tag>()
                .RuleFor(tag => tag.Name, (faker, tag) => faker.Company.Random.Word());
            return factory.Generate(25).ToArray();
        }

        #endregion
    }
}