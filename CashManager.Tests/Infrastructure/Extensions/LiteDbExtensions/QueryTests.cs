using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Extensions.LiteDbExtensions
{
    public class QueryTests
    {
        [Fact]
        public void Query_EmptyDb_Empty()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();

            //when
            var result = repo.Database.Query<Dto>();

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Query_NotEmptyDb_Same()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[] { new Tag(), new Tag() };
            repo.Database.UpsertBulk(input);

            //when
            var result = repo.Database.Query<Tag>();

            //then
            Assert.Equal(input.OrderBy(x => x.Id), result.OrderBy(x => x.Id));
        }

        [Fact]
        public void Query_NotEmptyDbFilter_FilteredArray()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[]
            {
                new Tag { Name = "a"},
                new Tag { Name = "b"},
                new Tag { Name = "c"},
                new Tag(),
                new Tag(),
            };
            repo.Database.UpsertBulk(input);

            //when
            var result = repo.Database.Query<Tag>(x => !string.IsNullOrEmpty(x.Name));

            //then
            Assert.Equal(input.Where(x => !string.IsNullOrEmpty(x.Name)).OrderBy(x => x.Id), result.OrderBy(x => x.Id));
        }
    }
}