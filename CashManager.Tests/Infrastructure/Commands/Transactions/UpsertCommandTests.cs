using System.Collections.Generic;
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
            var command = new UpsertTransactionsCommand((Transaction)null);

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
                new Transaction { Note = "test1", Positions = new List<Position> { new Position { Title = "p1" } } }
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
            var tags = new List<Tag> { new Tag(), new Tag() };
            var transactions = new[]
            {
                new Transaction
                {
                    Note = "test1",
                    Positions = new List<Position> { new Position { Title = "p1", Category = new Category(), Tags = tags } }
                },
                new Transaction { Note = "test2", Positions = new List<Position> { new Position { Title = "p2", Category = new Category() } } }
            };
            var positions = transactions.SelectMany(x => x.Positions).OrderBy(x => x.Id).ToArray();
            var categories = transactions.SelectMany(x => x.Positions).Select(x => x.Category).OrderBy(x => x.Id).ToArray();

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionsCommandHandler(repository);
            var command = new UpsertTransactionsCommand(transactions);

            //these already should exist in db
            repository.Database.UpsertBulk(tags.ToArray());
            repository.Database.UpsertBulk(categories);

            //when
            handler.Execute(command);

            //then
            var orderedTransactionsInDatabase = repository.Database.Query<Transaction>().OrderBy(x => x.Id);
            Assert.Equal(transactions.OrderBy(x => x.Id), orderedTransactionsInDatabase);

            var actualPositions = repository.Database.Query<Position>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(positions, actualPositions);
            Assert.Equal(positions.Select(x => x.Title), actualPositions.Select(x => x.Title));
            Assert.Equal(positions.Select(x => x.Value.Value), actualPositions.Select(x => x.Value.Value));

            var orderedTags = positions.Where(x => x.Tags != null).SelectMany(x => x.Tags).OrderBy(x => x.Id).ToArray();
            var actualOrderedTags = actualPositions.Where(x => x.Tags != null).SelectMany(x => x.Tags).OrderBy(x => x.Id).ToArray();
            Assert.Equal(orderedTags, actualOrderedTags);
            Assert.Equal(orderedTags.Select(x => x.Name), actualOrderedTags.Select(x => x.Name));

            var actualCategories = repository.Database.Query<Category>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(categories, actualCategories);
            Assert.Equal(categories.Select(x => x.Value), actualCategories.Select(x => x.Value));
            Assert.Equal(categories.Select(x => x.Parent), actualCategories.Select(x => x.Parent));
        }

        [Fact]
        public void UpsertTransactionCommandHandler_NotEmptyDbUpsertList_ListUpdated()
        {
            //given
            var tags = new List<Tag> { new Tag(), new Tag() };
            var transactions = new[]
            {
                new Transaction
                {
                    Note = "test1",
                    Positions = new List<Position>
                    {
                        new Position
                        {
                            Title = "p1",
                            Category = new Category(),
                            Tags = tags,
                            Value = new PaymentValue { Value = 123.45 }
                        },
                        new Position
                        {
                            Title = "p2",
                            Category = new Category(),
                            Tags = tags,
                            Value = new PaymentValue { Value = 234.56 }
                        }
                    }
                },
                new Transaction { Note = "test2", Positions = new List<Position> { new Position { Title = "p3", Category = new Category() } } }
            };
            var positions = transactions.SelectMany(x => x.Positions).OrderBy(x => x.Id).ToArray();
            var categories = transactions.SelectMany(x => x.Positions).Select(x => x.Category).OrderBy(x => x.Id).ToArray();

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTransactionsCommandHandler(repository);
            var command = new UpsertTransactionsCommand(transactions);

            //these already should exist in db
            repository.Database.UpsertBulk(tags.ToArray());
            repository.Database.UpsertBulk(categories);
            repository.Database.UpsertBulk(transactions);

            foreach (var transaction in transactions) transaction.Note += " - updated";
            foreach (var position in positions) position.Value.Value += 1.0;

            //when
            handler.Execute(command);

            //then
            var orderedTransactionsInDatabase = repository.Database.Query<Transaction>().OrderBy(x => x.Id).ToArray();
            transactions = transactions.OrderBy(x => x.Id).ToArray();
            Assert.Equal(transactions, orderedTransactionsInDatabase);
            for (int i = 0; i < transactions.Length; i++) Assert.Equal(transactions[i].Note, orderedTransactionsInDatabase[i].Note);

            var actualPositions = repository.Database.Query<Position>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(positions, actualPositions);
            Assert.Equal(positions.Select(x => x.Title), actualPositions.Select(x => x.Title));
            Assert.Equal(positions.Select(x => x.Value.Value).ToArray(), actualPositions.Select(x => x.Value.Value).ToArray());

            var orderedTags = positions.Where(x => x.Tags != null).SelectMany(x => x.Tags).OrderBy(x => x.Id).ToArray();
            var actualOrderedTags = actualPositions.Where(x => x.Tags != null).SelectMany(x => x.Tags).OrderBy(x => x.Id).ToArray();
            Assert.Equal(orderedTags, actualOrderedTags);
            Assert.Equal(orderedTags.Select(x => x.Name), actualOrderedTags.Select(x => x.Name));

            var actualCategories = repository.Database.Query<Category>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(categories, actualCategories);
            Assert.Equal(categories.Select(x => x.Value), actualCategories.Select(x => x.Value));
            Assert.Equal(categories.Select(x => x.Parent), actualCategories.Select(x => x.Parent));
        }
    }
}