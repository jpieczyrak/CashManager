using System;
using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class FiltersTests : ViewModelTests
    {
        [Fact]
        public void OnBookDateFilterChanged_SomeTransactionsFilterNotEnabled_AllTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();

            //when
            vm.BookDateFilter.From = DateTime.Today;

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
            var expected = vm.Transactions
                             .Where(x => x.BookDate >= minDateTime && x.BookDate <= maxDateTime)
                             .OrderBy(x => x.BookDate)
                             .ToArray();

            //when
            vm.BookDateFilter.From = minDateTime;
            vm.BookDateFilter.To = maxDateTime;
            vm.BookDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.BookDate).ToArray());
        }

        [Fact]
        public void OnLastEditDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(-1);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            var expected = vm.Transactions
                             .Where(x => x.LastEditDate >= minDateTime && x.LastEditDate <= maxDateTime)
                             .OrderBy(x => x.LastEditDate)
                             .ToArray();

            //when
            vm.LastEditDateFilter.From = minDateTime;
            vm.LastEditDateFilter.To = maxDateTime;
            vm.LastEditDateFilter.IsChecked = true;

            //then
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.LastEditDate).ToArray());
        }

        [Fact]
        public void OnCreateDateFilterChanged_SomeTransactionsFilterEnabled_MatchingTransactions()
        {
            //given
            var minDateTime = DateTime.Today.AddDays(-10);
            var maxDateTime = DateTime.Today.AddDays(10);
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            var expected = vm.Transactions
                             .Where(x => x.InstanceCreationDate >= minDateTime && x.InstanceCreationDate <= maxDateTime)
                             .OrderBy(x => x.InstanceCreationDate)
                             .ToArray();

            //when
            vm.CreateDateFilter.From = minDateTime;
            vm.CreateDateFilter.To = maxDateTime;
            vm.CreateDateFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.InstanceCreationDate).ToArray());
        }
    }
}