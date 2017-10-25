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
            var subtransactions = new List<Subtransaction>
            {
                new Subtransaction("Sub 1", 12.32, "cat1", "tag1"),
                new Subtransaction("Sub 2", 1.32, "cat2", "tag1")
            };

            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note", DateTime.Now, DateTime.Today,
                subtransactions, null);
            var dto = Mapper.Map<Logic.DTO.Transaction>(expected);

            string serializedObject = Serializer.XMLSerializeObject(dto);
            var deserialized = Deserializer.Deserialize(serializedObject, typeof(Logic.DTO.Transaction));
            var mappedAfterDeserialization = Mapper.Map<Transaction>(deserialized);

            Assert.AreEqual(expected, mappedAfterDeserialization);

            Assert.AreEqual(expected.CreationDate, mappedAfterDeserialization.CreationDate);
            Assert.AreEqual(expected.Date, mappedAfterDeserialization.Date);
            Assert.AreEqual(expected.Note, mappedAfterDeserialization.Note);
            Assert.AreEqual(expected.Title, mappedAfterDeserialization.Title);

            CollectionAssert.AreEquivalent(expected.Subtransactions, mappedAfterDeserialization.Subtransactions);
            Assert.AreEqual(expected.Payment, mappedAfterDeserialization.Payment);
        }

        [Test]
        public void DatabaseTests()
        {
            var subtransactions = new List<Subtransaction>
            {
                new Subtransaction("Sub 1", 12.32, "cat1", "tag1"),
                new Subtransaction("Sub 2", 1.32, "cat2", "tag1")
            };

            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note", DateTime.Now, DateTime.Today,
                subtransactions, new Payment(StockProvider.Default, new Stock("Asd"), 12.4));
            var dto = Mapper.Map<Logic.DTO.Transaction>(expected);

            //string serializedObject = Serializer.XMLSerializeObject(dto);
            DatabaseProvider.DB.Update(dto);
            var loaded = DatabaseProvider.DB.Read<Logic.DTO.Transaction>().FirstOrDefault(t => t.Id == dto.Id);
            var mappedAfterDeserialization = Mapper.Map<Transaction>(loaded);

            Assert.AreEqual(expected, mappedAfterDeserialization);

            //we lose some accuracy in db
            Assert.AreEqual(expected.CreationDate.Date, mappedAfterDeserialization.CreationDate.Date);
            Assert.AreEqual(expected.Date.Date, mappedAfterDeserialization.Date.Date);

            Assert.AreEqual(expected.Note, mappedAfterDeserialization.Note);
            Assert.AreEqual(expected.Title, mappedAfterDeserialization.Title);

            CollectionAssert.AreEquivalent(expected.Subtransactions, mappedAfterDeserialization.Subtransactions);
            Assert.AreEqual(expected.Payment, mappedAfterDeserialization.Payment);
        }
    }
}