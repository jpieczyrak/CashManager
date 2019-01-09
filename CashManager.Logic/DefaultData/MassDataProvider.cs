using System;
using System.Linq;

using Bogus;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public class MassDataProvider : IDataProvider
    {
        private readonly TestDataProvider _test;

        public MassDataProvider()
        {
            Randomizer.Seed = new Random(123321);
            _test = new TestDataProvider();
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
                    positionsFactory.Generate(faker.Random.Int(1, 3)),
                    faker.PickRandom(userStocks),
                    faker.PickRandom(externalStocks),
                    $"input source {counter++}"));
            return factory.Generate(250).ToArray();
        }

        public Stock[] GetStocks()
        {
            var factory = new Faker<Stock>()
                .RuleFor(stock => stock.Name, (faker, stock) => faker.Company.CompanySuffix())
                .RuleFor(stock => stock.IsUserStock, faker => faker.Random.Bool())
                .RuleFor(stock => stock.LastEditDate, faker => faker.Date.Recent())
                .RuleFor(stock => stock.Balance, (faker, stock) => new Balance(faker.Date.Recent(30), faker.Random.Decimal(10m, 1000000m)));
            return factory.Generate(5).ToArray();
        }

        public Tag[] GetTags()
        {
            var factory = new Faker<Tag>()
                .RuleFor(tag => tag.Name, (faker, tag) => faker.Company.Random.Word());
            return factory.Generate(25).ToArray();
        }

        public TransactionType[] GetTransactionTypes() => _test.GetTransactionTypes();
        public Category[] GetCategories() => _test.GetCategories();


        #endregion
    }
}