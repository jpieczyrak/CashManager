using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.TransactionTypes;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.TransactionTypes
{
    public class UpsertCommandTests
    {
        [Fact]
        public void UpsertTransactionCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionTypesCommandHandler(repository);
            var command = new UpsertTransactionTypesCommand((TransactionType)null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Transaction>());
        }

        [Fact]
        public void UpsertTransactionCommandHandler_EmptyDbUpsertOneObject_ObjectSaved()
        {
            //given
            var transactionTypes = new[]
            {
                new TransactionType { Name = "t1", Outcome = true }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionTypesCommandHandler(repository);
            var command = new UpsertTransactionTypesCommand(transactionTypes);

            //when
            handler.Execute(command);

            //then
            var orderedTransactionTypesInDatabase = repository.Database.Query<TransactionType>().OrderBy(x => x.Id);
            Assert.Equal(transactionTypes.OrderBy(x => x.Id), orderedTransactionTypesInDatabase);
        }

        [Fact]
        public void UpsertTransactionCommandHandler_EmptyDbUpsertList_ListSaved()
        {
            //given
            var transactionTypes = new[]
            {
                new TransactionType { Name = "t1", Outcome = true }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionTypesCommandHandler(repository);
            var command = new UpsertTransactionTypesCommand(transactionTypes);

            //when
            handler.Execute(command);

            //then
            var orderedTransactionTypesInDatabase = repository.Database.Query<TransactionType>().OrderBy(x => x.Id).ToArray();
            var orderedTransactionTypes = transactionTypes.OrderBy(x => x.Id).ToArray();
            Assert.Equal(orderedTransactionTypes, orderedTransactionTypesInDatabase);
            Assert.Equal(orderedTransactionTypes.Select(x => x.Name), orderedTransactionTypesInDatabase.Select(x => x.Name));
            Assert.Equal(orderedTransactionTypes.Select(x => x.Id), orderedTransactionTypesInDatabase.Select(x => x.Id));
            Assert.Equal(orderedTransactionTypes.Select(x => x.IsDefault), orderedTransactionTypesInDatabase.Select(x => x.IsDefault));
            Assert.Equal(orderedTransactionTypes.Select(x => x.Income), orderedTransactionTypesInDatabase.Select(x => x.Income));
            Assert.Equal(orderedTransactionTypes.Select(x => x.Outcome), orderedTransactionTypesInDatabase.Select(x => x.Outcome));
        }

        [Fact]
        public void UpsertTransactionCommandHandler_NotEmptyDbUpsertList_ListUpdated()
        {
            //given
            var transactionTypes = new[]
            {
                new TransactionType { Name = "t1", Outcome = true },
                new TransactionType { Name = "t2", Income = true },
                new TransactionType { Name = "t2", Income = true, IsDefault = true },
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionTypesCommandHandler(repository);
            var command = new UpsertTransactionTypesCommand(transactionTypes);

            foreach (var transaction in transactionTypes)
            {
                transaction.Name += " - updated";
                transaction.Income = !transaction.Income;
                transaction.Outcome = !transaction.Outcome;
            }

            //when
            handler.Execute(command);

            //then
            var orderedTransactionTypesInDatabase = repository.Database.Query<TransactionType>().OrderBy(x => x.Id).ToArray();
            var orderedTransactionTypes = transactionTypes.OrderBy(x => x.Id).ToArray();
            Assert.Equal(orderedTransactionTypes, orderedTransactionTypesInDatabase);
            Assert.Equal(orderedTransactionTypes.Select(x => x.Name), orderedTransactionTypesInDatabase.Select(x => x.Name));
            Assert.Equal(orderedTransactionTypes.Select(x => x.Id), orderedTransactionTypesInDatabase.Select(x => x.Id));
            Assert.Equal(orderedTransactionTypes.Select(x => x.IsDefault), orderedTransactionTypesInDatabase.Select(x => x.IsDefault));
            Assert.Equal(orderedTransactionTypes.Select(x => x.Income), orderedTransactionTypesInDatabase.Select(x => x.Income));
            Assert.Equal(orderedTransactionTypes.Select(x => x.Outcome), orderedTransactionTypesInDatabase.Select(x => x.Outcome));
        }
    }
}