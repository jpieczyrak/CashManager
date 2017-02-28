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
            var subtransactions = new List<Subtransaction>
            {
                new Subtransaction("Sub 1", 12.32, "cat1", "tag1"),
                new Subtransaction("Sub 2", 1.32, "cat2", "tag1")
            };
            var transactionPartPayments = new List<TransactionPartPayment>
            {
                new TransactionPartPayment(Stock.Unknown, 1.23, ePaymentType.Value),
                new TransactionPartPayment(Stock.Unknown, 12.3, ePaymentType.Value)
            };

            var expected = new Transaction(eTransactionType.Buy, DateTime.Now, "title", "note", Stock.Unknown, DateTime.Now, DateTime.Today,
                subtransactions, transactionPartPayments);
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
            CollectionAssert.AreEquivalent(expected.TransactionSoucePayments, mappedAfterDeserialization.TransactionSoucePayments);
        }
    }
}