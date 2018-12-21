using System;
using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
{
    public class DateFrameTests : ViewModelTests
    {
        [Fact]
        public void OnBookDateFilterChanged_SomePositionsFilterNotEnabled_AllPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.BookDateFilter.From = DateTime.Today;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Length);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnBookDateFilterChanged_SomePositionsFilterEnabled_MatchingPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => x.BookDate >= minDateTime && x.BookDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.BookDateFilter.From = minDateTime;
            vm.SearchState.BookDateFilter.To = maxDateTime;
            vm.SearchState.BookDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Length);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnLastEditDateFilterChanged_SomePositionsFilterEnabled_MatchingPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => x.LastEditDate >= minDateTime && x.LastEditDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.LastEditDateFilter.From = minDateTime;
            vm.SearchState.LastEditDateFilter.To = maxDateTime;
            vm.SearchState.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Equal(expected.Length, vm.Positions.Length);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnCreateDateFilterChanged_SomePositionsFilterEnabled_MatchingPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-90);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => x.InstanceCreationDate >= minDateTime && x.InstanceCreationDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.CreateDateFilter.From = minDateTime;
            vm.SearchState.CreateDateFilter.To = maxDateTime;
            vm.SearchState.CreateDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Length);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }


        [Fact]
        public void OnBookDateFilterChanged_NotPositionSearchSomePositionsFilterNotEnabled_AllPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;

            //when
            vm.SearchState.BookDateFilter.From = DateTime.Today;

            //then
            Assert.Empty(vm.Positions);
        }

        [Fact]
        public void OnBookDateFilterChanged_NotPositionSearchSomePositionsFilterEnabled_AllPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;
            vm.IsTransactionsSearch = false;

            //when
            vm.SearchState.BookDateFilter.From = minDateTime;
            vm.SearchState.BookDateFilter.To = maxDateTime;
            vm.SearchState.BookDateFilter.IsChecked = true;

            //then
            Assert.Empty(vm.Positions);
        }

        [Fact]
        public void OnLastEditDateFilterChanged_NotPositionSearchSomePositionsFilterEnabled_AllPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;
            vm.IsTransactionsSearch = false;

            //when
            vm.SearchState.LastEditDateFilter.From = minDateTime;
            vm.SearchState.LastEditDateFilter.To = maxDateTime;
            vm.SearchState.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Empty(vm.Positions);
        }

        [Fact]
        public void OnCreateDateFilterChanged_NotPositionSearchSomePositionsFilterEnabled_AllPositions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsPositionsSearch = false;
            vm.IsTransactionsSearch = false;

            //when
            vm.SearchState.CreateDateFilter.From = minDateTime;
            vm.SearchState.CreateDateFilter.To = maxDateTime;
            vm.SearchState.CreateDateFilter.IsChecked = true;

            //then
            Assert.Empty(vm.Positions);
        }
    }
}