using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.TransactionTypes;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.TransactionTypes
{
    public class DeleteTransactionTypeCommandTests
    {
        [Fact]
        public void DeleteTransactionTypeCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionTypeCommandHandler(repository);
            var command = new DeleteTransactionTypeCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<TransactionType>());
        }

        [Fact]
        public void DeleteTransactionTypeCommandHandler_NotExisting_NoChange()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionTypeCommandHandler(repository);
            var command = new DeleteTransactionTypeCommand(new TransactionType());

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<TransactionType>());
        }

        [Fact]
        public void DeleteTransactionTypeCommandHandler_Existing_Removed()
        {
            //given
            var transactionType = new TransactionType();
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionTypeCommandHandler(repository);
            var command = new DeleteTransactionTypeCommand(transactionType);
            repository.Database.Upsert(transactionType);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<TransactionType>());
        }

        [Fact]
        public void DeleteTransactionTypeCommandHandler_MoreObjects_RemovedProperOne()
        {
            //given
            var targetTransactionType = new TransactionType();
            var transactionTypes = new[] { targetTransactionType, new TransactionType(), new TransactionType(), new TransactionType() };
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTransactionTypeCommandHandler(repository);
            var command = new DeleteTransactionTypeCommand(targetTransactionType);
            repository.Database.UpsertBulk(transactionTypes);

            //when
            handler.Execute(command);

            //then
            transactionTypes = transactionTypes.Skip(1).OrderBy(x => x.Id).ToArray();
            var actualTransactionTypes = repository.Database.Query<TransactionType>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(transactionTypes.Length, actualTransactionTypes.Length);
            Assert.Equal(transactionTypes, actualTransactionTypes);
        }
    }
}