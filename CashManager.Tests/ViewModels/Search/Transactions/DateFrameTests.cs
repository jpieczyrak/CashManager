using System;
using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class DateFrameTests : ViewModelTests
    {
        [Fact]
        public void OnBookDateFilterChanged_SomeTransactionsFilterNotEnabled_AllTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = true;

            //when
            vm.SearchState.BookDateFilter.From = DateTime.Today;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(Transactions.Length, vm.Transactions.Length);
        }

        [Fact]
        public void OnBookDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.BookDate >= minDateTime && x.BookDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.BookDateFilter.From = minDateTime;
            vm.SearchState.BookDateFilter.To = maxDateTime;
            vm.SearchState.BookDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnLastEditDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.LastEditDate >= minDateTime && x.LastEditDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.LastEditDateFilter.From = minDateTime;
            vm.SearchState.LastEditDateFilter.To = maxDateTime;
            vm.SearchState.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnCreateDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.InstanceCreationDate >= minDateTime && x.InstanceCreationDate <= maxDateTime)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.CreateDateFilter.From = minDateTime;
            vm.SearchState.CreateDateFilter.To = maxDateTime;
            vm.SearchState.CreateDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }


        [Fact]
        public void OnBookDateFilterChanged_NotTransactionSearchSomeTransactionsFilterNotEnabled_AllTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = false;

            //when
            vm.SearchState.BookDateFilter.From = DateTime.Today;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(Transactions.Length, vm.Transactions.Length);
        }

        [Fact]
        public void OnBookDateFilterChanged_NotTransactionSearchSomeTransactionsFilterEnabled_AllTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = false;
            var expected = vm.Transactions.ToArray();

            //when
            vm.SearchState.BookDateFilter.From = minDateTime;
            vm.SearchState.BookDateFilter.To = maxDateTime;
            vm.SearchState.BookDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.ToArray());
        }

        [Fact]
        public void OnLastEditDateFilterChanged_NotTransactionSearchSomeTransactionsFilterEnabled_AllTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = false;
            var expected = vm.Transactions.ToArray();

            //when
            vm.SearchState.LastEditDateFilter.From = minDateTime;
            vm.SearchState.LastEditDateFilter.To = maxDateTime;
            vm.SearchState.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.ToArray());
        }

        [Fact]
        public void OnCreateDateFilterChanged_NotTransactionSearchSomeTransactionsFilterEnabled_AllTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = false;
            var expected = vm.Transactions.ToArray();

            //when
            vm.SearchState.CreateDateFilter.From = minDateTime;
            vm.SearchState.CreateDateFilter.To = maxDateTime;
            vm.SearchState.CreateDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.ToArray());
        }
    }
}