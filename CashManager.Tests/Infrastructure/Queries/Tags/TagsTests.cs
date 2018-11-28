using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.Tags
{
    public class TagsTests
    {
        [Fact]
        public void TagQueryHandler_TagQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new TagQueryHandler(repository);
            var query = new TagQuery();

            //when
            var result = handler.Execute(query);

            //then
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TagQueryHandler_TagQueryNotEmptyDatabase_Array()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new TagQueryHandler(repository);
            var query = new TagQuery();
            var tags = new[]
            {
                new Tag { Name = "tag1"}, 
                new Tag { Name = "tag2"},
                new Tag { Name = "tag3"},
                new Tag { Name = "tag4"},
                new Tag { Name = "tag5"},
            };
            repository.Database.UpsertBulk(tags);

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(tags.OrderBy(x => x.Id), result.OrderBy(x => x.Id));
        }
    }
}