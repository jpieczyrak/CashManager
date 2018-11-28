using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Extensions
{
    public class LiteDbExtensionTests
    {
        [Fact]
        public void SingleUpsert_NotNullNotInDbAlready_TrueAndObjectInDb()
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
        public void SingleUpsert_NotNullAlreadyInDb_FalseAndUpdatedObjectInDb()
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
        public void UpsertBulk_NotNullNotInDbAlready_CountAndObjectInDb()
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
        public void UpsertBulk_NotNullAlreadyInDb_0AndObjectInDb()
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

        [Fact]
        public void Read_EmptyDb_Empty()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();

            //when
            var result = repo.Database.Read<Dto>();

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Read_NotEmptyDb_Same()
        {
            //given
            var repo = LiteDbHelper.CreateMemoryDb();
            var input = new[] { new Tag(), new Tag() };
            repo.Database.UpsertBulk(input);

            //when
            var result = repo.Database.Read<Tag>();

            //then
            Assert.Equal(input.OrderBy(x => x.Id), result.OrderBy(x => x.Id));
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
        public void Query_NotEmptyDb_FilteredArray()
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