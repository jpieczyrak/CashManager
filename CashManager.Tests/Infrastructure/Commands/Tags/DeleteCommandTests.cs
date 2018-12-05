using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Tags;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Tags
{
    public class DeleteCommandTests
    {
        [Fact]
        public void DeleteTagCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTagCommandHandler(repository);
            var command = new DeleteTagCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Tag>());
        }

        [Fact]
        public void DeleteTagCommandHandler_NotExisting_NoChange()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTagCommandHandler(repository);
            var command = new DeleteTagCommand(new Tag());

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Tag>());
        }

        [Fact]
        public void DeleteTagCommandHandler_Existing_Removed()
        {
            //given
            var tag = new Tag();
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTagCommandHandler(repository);
            var command = new DeleteTagCommand(tag);
            repository.Database.Upsert(tag);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Tag>());
        }

        [Fact]
        public void DeleteTagCommandHandler_MoreObjects_RemovedProperOne()
        {
            //given
            var targetTag = new Tag();
            var tags = new[] { targetTag, new Tag(), new Tag(), new Tag() };
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteTagCommandHandler(repository);
            var command = new DeleteTagCommand(targetTag);
            repository.Database.UpsertBulk(tags);

            //when
            handler.Execute(command);

            //then
            tags = tags.Skip(1).OrderBy(x => x.Id).ToArray();
            var actualTags = repository.Database.Query<Tag>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(tags.Length, actualTags.Length);
            Assert.Equal(tags, actualTags);
        }
    }
}