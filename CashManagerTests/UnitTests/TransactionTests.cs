using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Logic.Database;
using Logic.FilesOperations;
using Logic.IoC;
using Logic.LogicObjectsProviders;
using Logic.Model;
using Logic.TransactionManagement.TransactionElements;

using NUnit.Framework;

using MapperConfiguration = Logic.Mapping.MapperConfiguration;

namespace CashManagerTests.UnitTests
{
    [TestFixture]
    public class TransactionTests
    {
        [SetUp]
        public void Setup()
        {
            MapperConfiguration.Configure();
            AutofacConfiguration.UseStrategy(eBuildStrategyType.Test);
        }

        [Test]
        public void SerializationTests()
        {
            //given
            var subtransactions = new List<Subtransaction>
            {
                new Subtransaction("Sub 1", 12.32, "cat1"),
                new Subtransaction("Sub 2", 1.32, "cat2")
            };

            var myStock = new Stock("wallet");
            var externalStock = new Stock("shop");
            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note",
                subtransactions, myStock, externalStock, "input");
            var dto = Mapper.Map<Logic.DTO.Transaction>(expected);

            //when
            string serializedObject = Serializer.XMLSerializeObject(dto);
            var deserialized = Deserializer.Deserialize(serializedObject, typeof(Logic.DTO.Transaction));
            var mappedAfterDeserialization = Mapper.Map<Transaction>(deserialized);

            //then
            Assert.AreEqual(expected, mappedAfterDeserialization);
            
            Assert.AreEqual(expected.BookDate, mappedAfterDeserialization.BookDate);
            Assert.AreEqual(expected.Note, mappedAfterDeserialization.Note);
            Assert.AreEqual(expected.Title, mappedAfterDeserialization.Title);

            CollectionAssert.AreEquivalent(expected.Subtransactions, mappedAfterDeserialization.Subtransactions);
            Assert.AreEqual(expected.UserStock, mappedAfterDeserialization.UserStock);
            Assert.AreEqual(expected.ExternalStock, mappedAfterDeserialization.ExternalStock);
        }

        [Test]
        public void DatabaseTests()
        {
            //given
            var subtransactions = new List<Subtransaction>
            {
                new Subtransaction("Sub 1", 12.32, "cat1"),
                new Subtransaction("Sub 2", 1.32, "cat2")
            };

            var myStock = new Stock("wallet");
            var externalStock = new Stock("shop");
            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note",
                subtransactions, myStock, externalStock, "input");
            var dto = Mapper.Map<Logic.DTO.Transaction>(expected);

            //when
            DatabaseProvider.DB.Upsert(dto);
            var loaded = DatabaseProvider.DB.Read<Logic.DTO.Transaction>().FirstOrDefault(t => t.Id == dto.Id);
            var mappedAfterDeserialization = Mapper.Map<Transaction>(loaded);
            
            //then
            Assert.AreEqual(expected.Id, mappedAfterDeserialization.Id);
            Assert.AreEqual(expected, mappedAfterDeserialization);

            //we lose some accuracy in db
            Assert.AreEqual(expected.BookDate.Date, mappedAfterDeserialization.BookDate.Date);

            Assert.AreEqual(expected.Note, mappedAfterDeserialization.Note);
            Assert.AreEqual(expected.Title, mappedAfterDeserialization.Title);

            CollectionAssert.AreEquivalent(expected.Subtransactions, mappedAfterDeserialization.Subtransactions);
            Assert.AreEqual(expected.UserStock, mappedAfterDeserialization.UserStock);
            Assert.AreEqual(expected.ExternalStock, mappedAfterDeserialization.ExternalStock);
        }
    }
}