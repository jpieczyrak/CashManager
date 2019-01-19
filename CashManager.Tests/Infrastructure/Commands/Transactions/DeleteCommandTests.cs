using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Transactions
{
    public class DeleteTransactionCommandTests
    {
        [Fact]
        public void DeleteTransactionCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionCommandHandler(repository);
            var command = new DeleteTransactionCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Transaction>());
        }

        [Fact]
        public void DeleteTransactionCommandHandler_NotExisting_NoChange()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionCommandHandler(repository);
            var command = new DeleteTransactionCommand(new Transaction());

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Transaction>());
        }

        [Fact]
        public void DeleteTransactionCommandHandler_Existing_Removed()
        {
            //given
            var transaction = new Transaction();
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionCommandHandler(repository);
            var command = new DeleteTransactionCommand(transaction);
            repository.Database.Upsert(transaction);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Transaction>());
        }

        [Fact]
        public void DeleteTransactionCommandHandler_MoreObjects_RemovedProperOne()
        {
            //given
            var targetTransaction = new Transaction();
            var transactions = new[] { targetTransaction, new Transaction(), new Transaction(), new Transaction() };
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionCommandHandler(repository);
            var command = new DeleteTransactionCommand(targetTransaction);
            repository.Database.UpsertBulk(transactions);

            //when
            handler.Execute(command);

            //then
            transactions = transactions.Skip(1).OrderBy(x => x.Id).ToArray();
            var actualTransactions = repository.Database.Query<Transaction>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(transactions.Length, actualTransactions.Length);
            Assert.Equal(transactions, actualTransactions);
        }
    }
}