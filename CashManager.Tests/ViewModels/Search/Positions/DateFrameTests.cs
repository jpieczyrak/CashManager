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
            vm.State.BookDateFilter.From = DateTime.Today;

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
            vm.State.BookDateFilter.From = minDateTime;
            vm.State.BookDateFilter.To = maxDateTime;
            vm.State.BookDateFilter.IsChecked = true;

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
            vm.State.LastEditDateFilter.From = minDateTime;
            vm.State.LastEditDateFilter.To = maxDateTime;
            vm.State.LastEditDateFilter.IsChecked = true;

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
            vm.State.CreateDateFilter.From = minDateTime;
            vm.State.CreateDateFilter.To = maxDateTime;
            vm.State.CreateDateFilter.IsChecked = true;

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
            vm.State.BookDateFilter.From = DateTime.Today;

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
            vm.State.BookDateFilter.From = minDateTime;
            vm.State.BookDateFilter.To = maxDateTime;
            vm.State.BookDateFilter.IsChecked = true;

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
            vm.State.LastEditDateFilter.From = minDateTime;
            vm.State.LastEditDateFilter.To = maxDateTime;
            vm.State.LastEditDateFilter.IsChecked = true;

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
            vm.State.CreateDateFilter.From = minDateTime;
            vm.State.CreateDateFilter.To = maxDateTime;
            vm.State.CreateDateFilter.IsChecked = true;

            //then
            Assert.Empty(vm.Positions);
        }
    }
}