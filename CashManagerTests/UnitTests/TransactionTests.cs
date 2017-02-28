using System;
using System.Collections.Generic;

using AutoMapper;

using Logic.FilesOperations;
using Logic.Model;
using Logic.StocksManagement;
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
        }

        [Test]
        public void SerializationTests()
        {
            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note", Stock.Unknown, DateTime.Now, DateTime.Today,
                new List<Subtransaction>(), new List<TransactionPartPayment>());
            var dto = Mapper.Map<Logic.DTO.Transaction>(expected);

            string serializedObject = Serializer.XMLSerializeObject(dto);
            var deserialized = Deserializer.Deserialize(serializedObject, typeof(Logic.DTO.Transaction));
            var mappedAfterDeserialization = Mapper.Map<Transaction>(deserialized);

            Console.WriteLine(expected.Id);
            Console.WriteLine(mappedAfterDeserialization.Id);

            Assert.AreEqual(expected, mappedAfterDeserialization);

            Assert.AreEqual(expected.CreationDate, mappedAfterDeserialization.CreationDate);
            Assert.AreEqual(expected.Date, mappedAfterDeserialization.Date);
            Assert.AreEqual(expected.Note, mappedAfterDeserialization.Note);
            Assert.AreEqual(expected.Title, mappedAfterDeserialization.Title);
        }
    }
}