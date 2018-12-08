using System;
using System.Linq;

using CashManager.Data.DTO;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class BaseParserTests
    {
        protected static void ValidateTransaction(Transaction result, Transaction expected)
        {
            Assert.NotNull(result);
            Assert.Equal(expected.BookDate, result.BookDate);
            Assert.Equal(expected.Title, result.Title);
            Assert.Equal(expected.Note, result.Note);
            Assert.Equal(expected.UserStock, result.UserStock);
            Assert.Equal(expected.ExternalStock, result.ExternalStock);

            Assert.Equal(expected.BookDate, result.BookDate);
            Assert.Equal(expected.TransactionSourceCreationDate, result.TransactionSourceCreationDate);

            var instanceCreationDiff = expected.InstanceCreationDate - result.InstanceCreationDate;
            Assert.InRange(instanceCreationDiff, TimeSpan.FromSeconds(-1), TimeSpan.FromSeconds(1));

            Assert.Equal(expected.Positions.First().Title, result.Positions.First().Title);
            Assert.Equal(expected.Positions.First().Value.GrossValue, result.Positions.First().Value.GrossValue);

            Assert.Equal(expected.Positions.Sum(x => x.Value.GrossValue), result.Positions.Sum(x => x.Value.GrossValue));
        }
    }
}