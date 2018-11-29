using System;
using System.Linq;
using System.Linq.Expressions;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.TransactionTypes
{
    public class TransactionTypesTests
    {
        [Fact]
        public void TransactionTypesQueryHandler_TransactionTypesQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new TransactionTypesQueryHandler(repository);
            var query = new TransactionTypesQuery();

            //when
            var result = handler.Execute(query);

            //then
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TransactionTypesQueryHandler_TransactionTypesQueryNotEmptyDatabaseWithNoQuery_AllTransactionTypes()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new TransactionTypesQueryHandler(repository);
            var query = new TransactionTypesQuery();
            var transactions = new[]
            {
                new TransactionType(), 
                new TransactionType(), 
            };
            repository.Database.GetCollection<TransactionType>().InsertBulk(transactions);

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(transactions, result);
        }

        [Fact]
        public void TransactionTypesQueryHandler_TransactionTypesQueryNotEmptyDatabaseWithQuery_MatchingArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new TransactionTypesQueryHandler(repository);
            Expression<Func<TransactionType, bool>> linqQuery = x => x.IsDefault && x.Outcome;
            var query = new TransactionTypesQuery(linqQuery);
            var transactions = new[]
            {
                new TransactionType { Income = true, IsDefault = true, Name = "income"},
                new TransactionType { Outcome = true, IsDefault = true, Name = "outcome"},
            };
            repository.Database.GetCollection<TransactionType>().InsertBulk(transactions);
            var matchingTransactions = transactions.AsQueryable().Where(linqQuery).ToArray();

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(matchingTransactions.Length, result.Length);
            Assert.Equal(matchingTransactions, result);
        }
    }
}