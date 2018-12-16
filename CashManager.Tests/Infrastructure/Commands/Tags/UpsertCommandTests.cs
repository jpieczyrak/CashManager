using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Tags;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Tags
{
    public class UpsertCommandTests
    {
        [Fact]
        public void UpsertTagCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTagsCommandHandler(repository);
            var command = new UpsertTagsCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Tag>());
        }

        [Fact]
        public void UpsertTagCommandHandler_EmptyDbUpsertOneObject_ObjectSaved()
        {
            //given
            var tags = new[]
            {
                new Tag { Name = "test1" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTagsCommandHandler(repository);
            var command = new UpsertTagsCommand(tags);

            //when
            handler.Execute(command);

            //then
            var orderedTagsInDatabase = repository.Database.Query<Tag>().OrderBy(x => x.Id);
            Assert.Equal(tags.OrderBy(x => x.Id), orderedTagsInDatabase);
        }

        [Fact]
        public void UpsertTagCommandHandler_EmptyDbUpsertList_ListSaved()
        {
            //given
            var tags = new[]
            {
                new Tag { Name = "test1" },
                new Tag { Name = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTagsCommandHandler(repository);
            var command = new UpsertTagsCommand(tags);

            //when
            handler.Execute(command);

            //then
            var orderedTagsInDatabase = repository.Database.Query<Tag>().OrderBy(x => x.Id);
            Assert.Equal(tags.OrderBy(x => x.Id), orderedTagsInDatabase);
        }

        [Fact]
        public void UpsertTagCommandHandler_NotEmptyDbUpsertList_ListUpdated()
        {
            //given
            var tags = new[]
            {
                new Tag { Name = "test1" },
                new Tag { Name = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertTagsCommandHandler(repository);
            var command = new UpsertTagsCommand(tags);
            repository.Database.UpsertBulk(tags);
            foreach (var Tag in tags) Tag.Name += " - updated";

            //when
            handler.Execute(command);

            //then
            var orderedTagsInDatabase = repository.Database.Query<Tag>().OrderBy(x => x.Id).ToArray();
            tags = tags.OrderBy(x => x.Id).ToArray();
            Assert.Equal(tags, orderedTagsInDatabase);
            for (int i = 0; i < tags.Length; i++) Assert.Equal(tags[i].Name, orderedTagsInDatabase[i].Name);
        }
    }
}