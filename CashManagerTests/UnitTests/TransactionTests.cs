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
                new Subtransaction("Sub 1", 12.32, "cat1", "tag1"),
                new Subtransaction("Sub 2", 1.32, "cat2", "tag1")
            };

            var myStock = new Stock("wallet");
            var externalStock = new Stock("shop");
            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note", DateTime.Now, DateTime.Today,
                subtransactions, myStock, externalStock);
            var dto = Mapper.Map<Logic.DTO.Transaction>(expected);

            //when
            string serializedObject = Serializer.XMLSerializeObject(dto);
            var deserialized = Deserializer.Deserialize(serializedObject, typeof(Logic.DTO.Transaction));
            var mappedAfterDeserialization = Mapper.Map<Transaction>(deserialized);
            
            //then
            Assert.AreEqual(expected, mappedAfterDeserialization);

            Assert.AreEqual(expected.CreationDate, mappedAfterDeserialization.CreationDate);
            Assert.AreEqual(expected.Date, mappedAfterDeserialization.Date);
            Assert.AreEqual(expected.Note, mappedAfterDeserialization.Note);
            Assert.AreEqual(expected.Title, mappedAfterDeserialization.Title);

            CollectionAssert.AreEquivalent(expected.Subtransactions, mappedAfterDeserialization.Subtransactions);
            Assert.AreEqual(expected.MyStock, mappedAfterDeserialization.MyStock);
            Assert.AreEqual(expected.ExternalStock, mappedAfterDeserialization.ExternalStock);
        }

        [Test]
        public void DatabaseTests()
        {
            //given
            var subtransactions = new List<Subtransaction>
            {
                new Subtransaction("Sub 1", 12.32, "cat1", "tag1"),
                new Subtransaction("Sub 2", 1.32, "cat2", "tag1")
            };

            var myStock = new Stock("wallet");
            var externalStock = new Stock("shop");
            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note", DateTime.Now, DateTime.Today,
                subtransactions, myStock, externalStock);
            var dto = Mapper.Map<Logic.DTO.Transaction>(expected);

            //when
            DatabaseProvider.DB.Update(dto);
            var loaded = DatabaseProvider.DB.Read<Logic.DTO.Transaction>().FirstOrDefault(t => t.Id == dto.Id);
            var mappedAfterDeserialization = Mapper.Map<Transaction>(loaded);

            //then
            Assert.AreEqual(expected, mappedAfterDeserialization);

            //we lose some accuracy in db
            Assert.AreEqual(expected.CreationDate.Date, mappedAfterDeserialization.CreationDate.Date);
            Assert.AreEqual(expected.Date.Date, mappedAfterDeserialization.Date.Date);

            Assert.AreEqual(expected.Note, mappedAfterDeserialization.Note);
            Assert.AreEqual(expected.Title, mappedAfterDeserialization.Title);

            CollectionAssert.AreEquivalent(expected.Subtransactions, mappedAfterDeserialization.Subtransactions);
            Assert.AreEqual(expected.MyStock, mappedAfterDeserialization.MyStock);
            Assert.AreEqual(expected.ExternalStock, mappedAfterDeserialization.ExternalStock);
        }
    }
}