using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Extensions.LiteDbExtensions
{
    public class UpsertTests
    {
        [Fact]
        public void Upsert_NotNullNotInDbAlQuery_TrueAndObjectInDb()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var element = new Dto();

            //when
            bool result = repo.Database.Upsert(element);

            //then
            Assert.True(result);
            Assert.Equal(element, repo.Database.GetCollection<Dto>().FindAll().First());
        }

        [Fact]
        public void Upsert_NotNullAlQueryInDb_FalseAndUpdatedObjectInDb()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var element = new Tag { Name = "start name"};
            repo.Database.GetCollection<Tag>().Insert(element);

            //when
            element.Name = "edited";
            bool result = repo.Database.Upsert(element);

            //then
            Assert.False(result);
            var actual = repo.Database.GetCollection<Tag>().FindAll().First();
            Assert.Equal(element, actual);
            Assert.Equal(element.Name, actual.Name);
        }

        [Fact]
        public void UpsertBulk_Nulls_0()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            Dto[] input = { null, null, null };

            //when
            int result = repo.Database.UpsertBulk(input);

            //then
            Assert.Equal(0, result);
        }

        [Fact]
        public void UpsertBulk_NotNullNotInDbAlQuery_CountAndObjectInDb()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            Dto[] input = { new Tag { Name = "tag" }, new Dto(), new Dto() };

            //when
            int result = repo.Database.UpsertBulk(input);

            //then
            Assert.Equal(3, result);
            Assert.Equal(input.OrderBy(x => x.Id), repo.Database.GetCollection<Dto>().FindAll().OrderBy(x => x.Id));
        }

        [Fact]
        public void UpsertBulk_NotNullAlQueryInDb_0AndObjectInDb()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[]{ new Tag(), new Tag(), new Tag() };
            repo.Database.GetCollection<Tag>().Insert(input);

            //when
            foreach (var tag in input) tag.Name = "tag";
            int result = repo.Database.UpsertBulk(input);

            //then
            Assert.Equal(0, result);
            Assert.Equal(input.OrderBy(x => x.Id), repo.Database.GetCollection<Tag>().FindAll().OrderBy(x => x.Id));
            Assert.Equal(
                input.OrderBy(x => x.Id).Select(x => x.Name),
                repo.Database.GetCollection<Tag>().FindAll().OrderBy(x => x.Id).Select(x => x.Name));
        }
    }
}