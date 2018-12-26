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
            vm.Update();
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.ValueAsProfit <= maxValue && x.ValueAsProfit >= minValue)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.ValueFilter.Min = minValue;
            vm.State.ValueFilter.Max = maxValue;
            vm.State.ValueFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }
    }
}