using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class RangeTests : ViewModelTests
    {
        [Fact]
        public void OnRangeFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            decimal minValue = -250;
            decimal maxValue = 2450;
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.ValueAsProfit <= maxValue && x.ValueAsProfit >= minValue)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.ValueFilter.Min = minValue;
            vm.SearchState.ValueFilter.Max = maxValue;
            vm.SearchState.ValueFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }
    }
}