using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure
{
    public class DbMappingTests
    {
        [Fact]
        public void SimpleReferenceReadingTest()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var stock1 = new Stock();
            var stock2 = new Stock();
            var parentCategory = new Category { Value = "parent" };
            var category = new Category { Parent = parentCategory, Value = "child" };
            var paymentValue = new PaymentValue { Value = 123.45 };
            var tags = new List<Tag> { new Tag { Name = "a" }, new Tag { Name = "b" } };
            var positions = new List<Position>
            {
                new Position
                {
                    Category = category,
                    Value = paymentValue,
                    Title = "test",
                    Tags = tags
                }
            };
            var transaction = new Transaction
            {
                UserStock = stock1,
                ExternalStock = stock2,
                Positions = positions,
                BookDate = DateTime.Today,
                InstanceCreationDate = DateTime.Today,
                LastEditDate = DateTime.Today,
                TransactionSourceCreationDate = DateTime.Today
            };

            //when
            repo.Database.Upsert(transaction);
            repo.Database.UpsertBulk(new [] { category, parentCategory});
            repo.Database.UpsertBulk(new [] { stock1, stock2});
            repo.Database.UpsertBulk(tags.ToArray());
            repo.Database.UpsertBulk(positions.ToArray());
            repo.Database.Upsert(paymentValue);

            //then
            var actual = repo.Database.Query<Transaction>().First();
            Assert.StrictEqual(transaction, actual);

            Assert.Equal(transaction.ExternalStock, actual.ExternalStock);
            Assert.Equal(transaction.ExternalStock.Name, actual.ExternalStock.Name);
            Assert.Equal(transaction.ExternalStock.IsUserStock, actual.ExternalStock.IsUserStock);

            Assert.Equal(transaction.UserStock, actual.UserStock);
            Assert.Equal(transaction.UserStock.Name, actual.UserStock.Name);
            Assert.Equal(transaction.UserStock.IsUserStock, actual.UserStock.IsUserStock);

            Assert.Equal(transaction.Positions.First().Category, actual.Positions.First().Category);
            Assert.Equal(transaction.Positions.First().Category.Value, actual.Positions.First().Category.Value);
            Assert.Equal(transaction.Positions.First().Category.Parent, actual.Positions.First().Category.Parent);

            Assert.Equal(transaction.Positions.First().Tags, actual.Positions.First().Tags);

            Assert.Equal(transaction.Positions.First(), actual.Positions.First());
            Assert.Equal(transaction.Positions.First().Value, actual.Positions.First().Value);
            Assert.Equal(transaction.Positions.First().Value.Value, actual.Positions.First().Value.Value);

            Assert.Equal(transaction.Positions.First().Category.Parent, actual.Positions.First().Category.Parent);
            Assert.Equal(transaction.Positions.First().Category.Parent.Value, actual.Positions.First().Category.Parent.Value);
            Assert.Equal(transaction.Positions.First().Category.Parent.Parent, actual.Positions.First().Category.Parent.Parent);
        }

        [Fact]
        public void SimpleReferenceUpdateTest()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var stock1 = new Stock();
            var stock2 = new Stock();
            var parentCategory = new Category { Value = "parent" };
            var category = new Category { Parent = parentCategory, Value = "child" };
            var paymentValue = new PaymentValue { Value = 123.45 };
            var tags = new List<Tag> { new Tag { Name = "a" }, new Tag { Name = "b" } };
            var positions = new List<Position>
            {
                new Position
                {
                    Category = category,
                    Value = paymentValue,
                    Title = "test",
                    Tags = tags
                }
            };
            var transaction = new Transaction
            {
                UserStock = stock1,
                ExternalStock = stock2,
                Positions = positions,
                BookDate = DateTime.Today,
                InstanceCreationDate = DateTime.Today,
                LastEditDate = DateTime.Today,
                TransactionSourceCreationDate = DateTime.Today
            };

            //when
            repo.Database.Upsert(transaction);
            repo.Database.UpsertBulk(new [] { category, parentCategory});
            repo.Database.UpsertBulk(new [] { stock1, stock2});
            repo.Database.UpsertBulk(tags.ToArray());
            repo.Database.UpsertBulk(positions.ToArray());
            repo.Database.Upsert(paymentValue);

            //modify
            paymentValue.Value += 666.66;
            repo.Database.Upsert(paymentValue);

            transaction.ExternalStock.Name += "test";
            repo.Database.Upsert(transaction.ExternalStock);

            //then
            var actual = repo.Database.Query<Transaction>().First();
            Assert.StrictEqual(transaction, actual);

            Assert.Equal(transaction.ExternalStock, actual.ExternalStock);
            Assert.Equal(transaction.ExternalStock.Name, actual.ExternalStock.Name);
            Assert.Equal(transaction.ExternalStock.IsUserStock, actual.ExternalStock.IsUserStock);

            Assert.Equal(transaction.UserStock, actual.UserStock);
            Assert.Equal(transaction.UserStock.Name, actual.UserStock.Name);
            Assert.Equal(transaction.UserStock.IsUserStock, actual.UserStock.IsUserStock);

            Assert.Equal(transaction.Positions.First().Category, actual.Positions.First().Category);
            Assert.Equal(transaction.Positions.First().Category.Value, actual.Positions.First().Category.Value);
            Assert.Equal(transaction.Positions.First().Category.Parent, actual.Positions.First().Category.Parent);

            Assert.Equal(transaction.Positions.First().Tags, actual.Positions.First().Tags);

            Assert.Equal(transaction.Positions.First(), actual.Positions.First());
            Assert.Equal(transaction.Positions.First().Value, actual.Positions.First().Value);
            Assert.Equal(transaction.Positions.First().Value.Value, actual.Positions.First().Value.Value);

            Assert.Equal(transaction.Positions.First().Category.Parent, actual.Positions.First().Category.Parent);
            Assert.Equal(transaction.Positions.First().Category.Parent.Value, actual.Positions.First().Category.Parent.Value);
            Assert.Equal(transaction.Positions.First().Category.Parent.Parent, actual.Positions.First().Category.Parent.Parent);
        }

        /// <summary>
        /// Not supported yet...
        /// Check on: https://github.com/mbdavid/LiteDB/issues/808
        /// </summary>
        [Fact]
        public void VerySimpleCascadeSaveWithDbRefTest()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var category1 = new Category { Value = "parent" };
            var category2 = new Category { Value = "child", Parent = category1 };

            //when
            repo.Database.UpsertBulk(new [] { category2 });

            repo.Database.GetCollection<Category>().Upsert(category2);
            repo.Database.GetCollection<Category>().Update(category2);

            //then
            var loadedCategories = repo.Database.Query<Category>();
            Assert.Contains(category2, loadedCategories);
            Assert.Contains(category1, loadedCategories);
            Assert.Equal(category2, loadedCategories.First(x => x.Id == category2.Id));
            Assert.Equal(category1, loadedCategories.First(x => x.Id == category1.Id));
            int elementsCount = repo.Query<Category>().Count();
            Assert.Equal(2, elementsCount);
        }
    }
}