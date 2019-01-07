using System;
using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
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
        public void OnBookDateFilterChanged_SomeTransactionsFilterNotEnabled_AllTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = true;

            //when
            vm.State.BookDateFilter.From = DateTime.Today;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(_fixture.ViewModelTests.Transactions.Value.Length, vm.MatchingTransactions.Count);
        }

        [Fact]
        public void OnBookDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.BookDate >= minDateTime && x.BookDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.BookDateFilter.From = minDateTime;
            vm.State.BookDateFilter.To = maxDateTime;
            vm.State.BookDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnLastEditDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.LastEditDate >= minDateTime && x.LastEditDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.LastEditDateFilter.From = minDateTime;
            vm.State.LastEditDateFilter.To = maxDateTime;
            vm.State.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnCreateDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.InstanceCreationDate >= minDateTime && x.InstanceCreationDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.CreateDateFilter.From = minDateTime;
            vm.State.CreateDateFilter.To = maxDateTime;
            vm.State.CreateDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }


        [Fact]
        public void OnBookDateFilterChanged_NotTransactionSearchSomeTransactionsFilterNotEnabled_AllTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = false;

            //when
            vm.State.BookDateFilter.From = DateTime.Today;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(_fixture.ViewModelTests.Transactions.Value.Length, vm.MatchingTransactions.Count);
        }

        [Fact]
        public void OnBookDateFilterChanged_NotTransactionSearchSomeTransactionsFilterEnabled_AllTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = false;
            var expected = vm.MatchingTransactions.ToArray();

            //when
            vm.State.BookDateFilter.From = minDateTime;
            vm.State.BookDateFilter.To = maxDateTime;
            vm.State.BookDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.ToArray());
        }

        [Fact]
        public void OnLastEditDateFilterChanged_NotTransactionSearchSomeTransactionsFilterEnabled_AllTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = false;
            var expected = vm.MatchingTransactions.ToArray();

            //when
            vm.State.LastEditDateFilter.From = minDateTime;
            vm.State.LastEditDateFilter.To = maxDateTime;
            vm.State.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.ToArray());
        }

        [Fact]
        public void OnCreateDateFilterChanged_NotTransactionSearchSomeTransactionsFilterEnabled_AllTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsTransactionsSearch = false;
            var expected = vm.MatchingTransactions.ToArray();

            //when
            vm.State.CreateDateFilter.From = minDateTime;
            vm.State.CreateDateFilter.To = maxDateTime;
            vm.State.CreateDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.ToArray());
        }
    }
}