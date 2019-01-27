using System.Linq;

using AutoMapper;

using CashManager.Data.ViewModelState;
using CashManager.Infrastructure.Command.States;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.States
{
    public class UpsertCommandTests
    {
        [Fact]
        public void UpsertSearchStateCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertSearchStateCommandHandler(repository);
            var command = new UpsertSearchStateCommand((SearchState) null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<SearchState>());
        }

        [Fact]
        public void UpsertSearchStateCommandHandler_EmptyDbUpsertOneObjectWithoutName_ObjectSaved()
        {
            //given
            var searchState = new SearchState();

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertSearchStateCommandHandler(repository);
            var command = new UpsertSearchStateCommand(searchState);

            //when
            handler.Execute(command);

            //then
            var result = repository.Database.Query<SearchState>().ToArray();
            Assert.Single(result);
            Assert.Equal(searchState, result[0]);
        }

        [Fact]
        public void UpsertSearchStateCommandHandler_UpsertOneObjectWithoutNameNonEmptyDb_OneInstanceOfSavedObjectInDb()
        {
            //given
            CashManager.WPF.Configuration.Mapping.MapperConfiguration.Configure();
            var searchState = new CashManager.WPF.Features.Search.SearchState();
            var state = Mapper.Map<SearchState>(searchState);

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertSearchStateCommandHandler(repository);
            var command = new UpsertSearchStateCommand(state);

            repository.Database.Upsert(Mapper.Map<SearchState>(new CashManager.WPF.Features.Search.SearchState()));

            //when
            handler.Execute(command);

            //then
            var result = repository.Database.Query<SearchState>().ToArray();
            Assert.Single(result);
            Assert.Equal(state, result[0]);
        }

        [Fact]
        public void UpsertSearchStateCommandHandler_UpsertOneObjectWithoutNameTwice_OneInstanceOfSavedObjectInDb()
        {
            //given
            var searchState = new SearchState();

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertSearchStateCommandHandler(repository);
            var command = new UpsertSearchStateCommand(new [] { searchState, searchState } );

            //when
            handler.Execute(command);

            //then
            var result = repository.Database.Query<SearchState>().ToArray();
            Assert.Single(result);
            Assert.Equal(searchState, result[0]);
        }

        [Fact]
        public void UpsertSearchStateCommandHandler_EmptyDbUpsertListOfNamedObjects_ListSaved()
        {
            //given
            var searchStates = new[]
            {
                new SearchState { Name = "test1" },
                new SearchState { Name = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertSearchStateCommandHandler(repository);
            var command = new UpsertSearchStateCommand(searchStates);

            //when
            handler.Execute(command);

            //then
            var orderedSearchStatesInDatabase = repository.Database.Query<SearchState>().OrderBy(x => x.Id);
            Assert.Equal(searchStates.OrderBy(x => x.Id), orderedSearchStatesInDatabase);
        }

        [Fact]
        public void UpsertSearchStateCommandHandler_NotEmptyDbUpsertList_ListUpdated()
        {
            //given
            var searchStates = new[]
            {
                new SearchState { Name = "test1" },
                new SearchState { Name = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertSearchStateCommandHandler(repository);
            var command = new UpsertSearchStateCommand(searchStates);
            repository.Database.UpsertBulk(searchStates);
            foreach (var searchState in searchStates) searchState.Name += " - updated";

            //when
            handler.Execute(command);

            //then
            var orderedSearchStatesInDatabase = repository.Database.Query<SearchState>().OrderBy(x => x.Id).ToArray();
            searchStates = searchStates.OrderBy(x => x.Id).ToArray();
            Assert.Equal(searchStates, orderedSearchStatesInDatabase);
            for (int i = 0; i < searchStates.Length; i++) Assert.Equal(searchStates[i].Name, orderedSearchStatesInDatabase[i].Name);
        }
    }
}