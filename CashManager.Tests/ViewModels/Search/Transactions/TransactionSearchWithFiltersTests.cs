using System;
using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class TransactionSearchWithFiltersTests : ViewModelTests
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
    }
}