using System;
using System.Linq;
using System.Linq.Expressions;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.Transactions
{
    public class TransactionsTests
    {
        [Fact]
        public void TransactionQueryHandler_TransactionQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new TransactionQueryHandler(repository);
            var query = new TransactionQuery();

            //when
            var result = handler.Execute(query);

            //then
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TransactionQueryHandler_TransactionQueryNotEmptyDatabaseWithNoQuery_AllTransactions()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
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
            Assert.Equal(transactions.OrderBy(x => x.Id), result.OrderBy(x => x.Id));
        }

        [Fact]
        public void TransactionQueryHandler_TransactionQueryNotEmptyDatabaseWithQuery_MatchingArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new TransactionQueryHandler(repository);
            Expression<Func<Transaction, bool>> linqQuery = x => x.Title.Contains("1");
            var query = new TransactionQuery(linqQuery);
            var transactions = new[]
            {
                new Transaction(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1)) { Title = "t1" },
                new Transaction(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2)) { Title = "t2" }
            };
            repository.Database.GetCollection<Transaction>().InsertBulk(transactions);
            var matchingTransactions = transactions.AsQueryable().Where(linqQuery).ToArray();

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(matchingTransactions.Length, result.Length);
            Assert.Equal(matchingTransactions, result);
        }
    }
}