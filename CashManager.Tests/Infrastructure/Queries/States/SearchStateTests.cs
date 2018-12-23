using System.Linq;

using CashManager.Data.ViewModelState;
using CashManager.Infrastructure.Query.States;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.States
{
    public class SearchStateTests
    {
        [Fact]
        public void SearchStateQueryHandler_SearchStateQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new SearchStateQueryHandler(repository);
            var query = new SearchStateQuery();

            //when
            var result = handler.Execute(query);

            //then
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void SearchStateQueryHandler_SearchStateQueryNotEmptyDatabase_Array()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new SearchStateQueryHandler(repository);
            var query = new SearchStateQuery();
            var searchStates = new[]
            {
                new SearchState { Name = "test 1" },
                new SearchState { Name = "test 2" }
            };
            repository.Database.GetCollection<SearchState>().InsertBulk(searchStates);

            //when
            var result = handler.Execute(query);

            //then
            result = result.OrderBy(x => x.Name).ToArray();
            Assert.Equal(searchStates.OrderBy(x => x.Name).ToArray(), result);
            Assert.Equal(searchStates[0].Name, result[0].Name);
            Assert.Equal(searchStates[1].Name, result[1].Name);
        }
    }
} 