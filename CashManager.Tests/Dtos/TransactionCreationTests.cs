using System;

using CashManager.Data;
using CashManager.Data.DTO;

using Xunit;

namespace CashManager.Tests.Dtos
{
    public class TransactionCreationTests
    {
        [Fact]
        public void Constructor_Empty_AllDatesSetToNow()
        {
            //given
            var expectedLow = DateTime.Now;
            var expectedHigh = DateTime.Now.AddSeconds(1);

            //when
            var transaction = new Transaction();

            //then
            Assert.InRange(transaction.BookDate, expectedLow, expectedHigh);
            Assert.InRange(transaction.InstanceCreationDate, expectedLow, expectedHigh);
            Assert.InRange(transaction.LastEditDate, expectedLow, expectedHigh);
            Assert.InRange(transaction.TransactionSourceCreationDate, expectedLow, expectedHigh);
        }

        [Fact]
        public void Constructor_Id_AllDatesSetToNow()
        {
            //given
            var expectedLow = DateTime.Now;
            var expectedHigh = DateTime.Now.AddSeconds(1);

            //when
            var transaction = new Transaction(Guid.NewGuid());

            //then
            Assert.InRange(transaction.BookDate, expectedLow, expectedHigh);
            Assert.InRange(transaction.InstanceCreationDate, expectedLow, expectedHigh);
            Assert.InRange(transaction.LastEditDate, expectedLow, expectedHigh);
            Assert.InRange(transaction.TransactionSourceCreationDate, expectedLow, expectedHigh);
        }

        [Fact]
        public void Constructor_AllData_DatesMatchesSource()
        {
            //given
            var expectedLow = DateTime.Now;
            var expectedHigh = DateTime.Now.AddSeconds(1);
            var expectedSourceCreationDate = DateTime.Today.AddMilliseconds(-135489);

            //when
            var transaction = new Transaction(new TransactionType(), expectedSourceCreationDate, string.Empty, string.Empty, 
                new Position[0], null, null, string.Empty);

            //then
            Assert.Equal(expectedSourceCreationDate, transaction.BookDate);
            Assert.Equal(expectedSourceCreationDate, transaction.TransactionSourceCreationDate);
            Assert.InRange(transaction.InstanceCreationDate, expectedLow, expectedHigh);
            Assert.InRange(transaction.LastEditDate, expectedLow, expectedHigh);
        }

        [Fact]
        public void Constructor_AllDataSameInput_SameId()
        {
            //given
            string inputText = "Lorem ipsum dolor sit amet.";
            //when
            var transaction1 = new Transaction(new TransactionType(), DateTime.Today, string.Empty, string.Empty, 
                new Position[0], null, null, inputText);
            var transaction2 = new Transaction(new TransactionType(), DateTime.Today, string.Empty, string.Empty, 
                new Position[0], null, null, inputText);

            //then
            Assert.Equal(transaction1.Id, transaction2.Id);
        }

        [Fact]
        public void Constructor_AllDataNotSameInput_NotSameId()
        {
            //given
            string inputText1 = "Lorem ipsum dolor sit amet.";
            string inputText2 = inputText1 + 1;
            //when
            var transaction1 = new Transaction(new TransactionType(), DateTime.Today, string.Empty, string.Empty, 
                new Position[0], null, null, inputText1);
            var transaction2 = new Transaction(new TransactionType(), DateTime.Today, string.Empty, string.Empty, 
                new Position[0], null, null, inputText2);

            //then
            Assert.NotEqual(transaction1.Id, transaction2.Id);
        }
    }
}