﻿using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
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
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => x.Value.GrossValue <= maxValue && x.Value.GrossValue >= minValue)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.ValueFilter.Min = minValue;
            vm.SearchState.ValueFilter.Max = maxValue;
            vm.SearchState.ValueFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Length);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }
    }
}