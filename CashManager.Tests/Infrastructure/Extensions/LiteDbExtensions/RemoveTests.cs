using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Extensions.LiteDbExtensions
{
    public class RemoveTests
    {
        [Fact]
        public void Remove_Null_0()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();

            //when
            int result = repo.Database.Remove<Dto>(null);

            //then
            Assert.Equal(0, result);
        }

        [Fact]
        public void Remove_ElementNotInDb_0()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[] { new Tag(), new Tag() };
            repo.Database.UpsertBulk(input);

            //when
            int result = repo.Database.Remove(new Dto());

            //then
            Assert.Equal(0, result);
        }

        [Fact]
        public void Remove_ElementInDb_1()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[] { new Tag(), new Tag() };
            repo.Database.UpsertBulk(input);

            //when
            int result = repo.Database.Remove(input[0]);

            //then
            Assert.Equal(1, result);
        }

        [Fact]
        public void RemoveAll_ElementsInDb_ElementsCount()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[] { new Tag(), new Tag(), new Tag { Name = "Tag" } };
            repo.Database.UpsertBulk(input);

            //when
            int result = repo.Database.RemoveAll<Tag>();

            //then
            Assert.Equal(input.Length, result);
        }

        [Fact]
        public void RemoveAll_QueryElementsInDb_MatchingElementsCount()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[] { new Tag(), new Tag(), new Tag { Name = "Tag" } };
            repo.Database.UpsertBulk(input);

            //when
            int result = repo.Database.RemoveAll<Tag>(x => string.IsNullOrEmpty(x.Name));

            //then
            Assert.Equal(2, result);
        }
    }
}