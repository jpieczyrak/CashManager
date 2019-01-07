using System;
using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
{
    [Collection("Database collection")]
    public class DateFrameTests
    {
        private readonly DatabaseFixture _fixture;

        public DateFrameTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnBookDateFilterChanged_SomePositionsFilterNotEnabled_AllPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = false;
            vm.IsPositionsSearch = true;
            var expected = _fixture.ViewModelTests.Positions
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.BookDateFilter.From = DateTime.Today;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnBookDateFilterChanged_SomePositionsFilterEnabled_MatchingPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions
                             .Where(x => x.BookDate >= minDateTime && x.BookDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();
            vm.Update();

            //when
            vm.State.BookDateFilter.From = minDateTime;
            vm.State.BookDateFilter.To = maxDateTime;
            vm.State.BookDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnLastEditDateFilterChanged_SomePositionsFilterEnabled_MatchingPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions
                             .Where(x => x.LastEditDate >= minDateTime && x.LastEditDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();
            vm.Update();

            //when
            vm.State.LastEditDateFilter.From = minDateTime;
            vm.State.LastEditDateFilter.To = maxDateTime;
            vm.State.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnCreateDateFilterChanged_SomePositionsFilterEnabled_MatchingPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-90);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions
                             .Where(x => x.InstanceCreationDate >= minDateTime && x.InstanceCreationDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();
            vm.Update();

            //when
            vm.State.CreateDateFilter.From = minDateTime;
            vm.State.CreateDateFilter.To = maxDateTime;
            vm.State.CreateDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }


        [Fact]
        public void OnBookDateFilterChanged_NotPositionSearchSomePositionsFilterNotEnabled_Null()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;

            //when
            vm.State.BookDateFilter.From = DateTime.Today;

            //then
            Assert.Null(vm.MatchingPositions);
        }

        [Fact]
        public void OnBookDateFilterChanged_NotPositionSearchSomePositionsFilterEnabled_Null()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;
            vm.IsTransactionsSearch = false;

            //when
            vm.State.BookDateFilter.From = minDateTime;
            vm.State.BookDateFilter.To = maxDateTime;
            vm.State.BookDateFilter.IsChecked = true;

            //then
            Assert.Null(vm.MatchingPositions);
        }

        [Fact]
        public void OnLastEditDateFilterChanged_NotPositionSearchSomePositionsFilterEnabled_Null()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;
            vm.IsTransactionsSearch = false;

            //when
            vm.State.LastEditDateFilter.From = minDateTime;
            vm.State.LastEditDateFilter.To = maxDateTime;
            vm.State.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Null(vm.MatchingPositions);
        }

        [Fact]
        public void OnCreateDateFilterChanged_NotPositionSearchSomePositionsFilterEnabled_Null()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;
            vm.IsTransactionsSearch = false;

            //when
            vm.State.CreateDateFilter.From = minDateTime;
            vm.State.CreateDateFilter.To = maxDateTime;
            vm.State.CreateDateFilter.IsChecked = true;

            //then
            Assert.Null(vm.MatchingPositions);
        }
    }
}