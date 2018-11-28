using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Transactions
{
    public class UpsertCommandTests
    {
        [Fact]
        public void UpsertTransactionCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionsCommandHandler(repository);
            var command = new UpsertTransactionsCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Transaction>());
        }

        [Fact]
        public void UpsertTransactionCommandHandler_EmptyDbUpsertOneObject_ObjectSaved()
        {
            //given
            var transactions = new[]
            {
                new Transaction { Note = "test1" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionsCommandHandler(repository);
            var command = new UpsertTransactionsCommand(transactions);

            //when
            handler.Execute(command);

            //then
            var orderedTransactionsInDatabase = repository.Database.Query<Transaction>().OrderBy(x => x.Id);
            Assert.Equal(transactions.OrderBy(x => x.Id), orderedTransactionsInDatabase);
        }

        [Fact]
        public void UpsertTransactionCommandHandler_EmptyDbUpsertList_ListSaved()
        {
            //given
            var transactions = new[]
            {
                new Transaction { Note = "test1" },
                new Transaction { Note = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionsCommandHandler(repository);
            var command = new UpsertTransactionsCommand(transactions);

            //when
            handler.Execute(command);

            //then
            var orderedTransactionsInDatabase = repository.Database.Query<Transaction>().OrderBy(x => x.Id);
            Assert.Equal(transactions.OrderBy(x => x.Id), orderedTransactionsInDatabase);
        }

        [Fact]
        public void UpsertTransactionCommandHandler_NotEmptyDbUpsertList_ListUpdated()
        {
            //given
            var transactions = new[]
            {
                new Transaction { Note = "test1" },
                new Transaction { Note = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionsCommandHandler(repository);
            var command = new UpsertTransactionsCommand(transactions);
            repository.Database.UpsertBulk(transactions);
            foreach (var transaction in transactions) transaction.Note += " - updated";

            //when
            handler.Execute(command);

            //then
            var orderedTransactionsInDatabase = repository.Database.Query<Transaction>().OrderBy(x => x.Id).ToArray();
            transactions = transactions.OrderBy(x => x.Id).ToArray();
            Assert.Equal(transactions, orderedTransactionsInDatabase);
            for (int i = 0; i < transactions.Length; i++)
            {
                Assert.Equal(transactions[i].Note, orderedTransactionsInDatabase[i].Note);
            }
        }
    }
}