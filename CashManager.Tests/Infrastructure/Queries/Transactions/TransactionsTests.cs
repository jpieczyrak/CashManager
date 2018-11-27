using System;
using System.IO;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Query.Transactions;

using LiteDB;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.Transactions
{
    public class TransactionsTests
    {
        [Fact]
        public void TransactionQueryHandler_TransactionQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = GetEmptyDatabase();
            var handler = new TransactionQueryHandler(repository);
            var query = new TransactionQuery();

            //when
            var result = handler.Execute(query);

            //then
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TransactionQueryHandler_TransactionQueryNotEmptyDatabase_Array()
        {
            //given
            var repository = GetEmptyDatabase();
            var handler = new TransactionQueryHandler(repository);
            var query = new TransactionQuery();
            var transactions = new[]
            {
                new Transaction(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1)),
                new Transaction(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2))
            };
            repository.Database.GetCollection<Transaction>().InsertBulk(transactions);

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(transactions, result);
        }

        private static LiteRepository GetEmptyDatabase()
        {
            return new LiteRepository(new LiteDatabase(new MemoryStream()));
        }
    }
}