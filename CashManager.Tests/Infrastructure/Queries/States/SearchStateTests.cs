using System;
using System.Linq;

using CashManager.Data.ViewModelState;
using CashManager.Data.ViewModelState.Selectors;
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
                new SearchState
                {
                    Name = "test 1",
                    BookDateFilter = new DateFrameSelector
                    {
                        From = DateTime.Today,
                        To = DateTime.Today.AddDays(1),
                        Type = 1
                    },
                    CategoriesFilter = new MultiPicker
                    {
                        IsChecked = true,
                        Type = 4,
                        Selected = new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
                    }
                },
                new SearchState { Name = "test 2",  }
            };
            repository.Database.GetCollection<SearchState>().InsertBulk(searchStates);

            //when
            var result = handler.Execute(query);

            //then
            result = result.OrderBy(x => x.Name).ToArray();
            Assert.Equal(searchStates.OrderBy(x => x.Name).ToArray(), result);
            Assert.Equal(searchStates[0].Name, result[0].Name);
            Assert.Equal(searchStates[0].BookDateFilter.Type, result[0].BookDateFilter.Type);
            Assert.Equal(searchStates[0].BookDateFilter.From, result[0].BookDateFilter.From);
            Assert.Equal(searchStates[0].BookDateFilter.To, result[0].BookDateFilter.To);
            Assert.Equal(searchStates[0].BookDateFilter.IsChecked, result[0].BookDateFilter.IsChecked);
            Assert.Equal(searchStates[0].CategoriesFilter.Selected, result[0].CategoriesFilter.Selected);

            Assert.Equal(searchStates[1].Name, result[1].Name);
        }
    }
} 