using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
{
    [Collection("Database collection")]
    public class RangeTests
    {
        private readonly DatabaseFixture _fixture;

        public RangeTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnRangeFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            decimal minValue = -250;
            decimal maxValue = 2450;
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions.Value
                             .Where(x => x.Value.GrossValue <= maxValue && x.Value.GrossValue >= minValue)
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.ValueFilter.Min = minValue;
            vm.State.ValueFilter.Max = maxValue;
            vm.State.ValueFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }
    }
}