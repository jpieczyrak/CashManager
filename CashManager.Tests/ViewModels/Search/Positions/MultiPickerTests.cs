using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
{
    [Collection("Database collection")]
    public class MultiPickerTests
    {
        private readonly DatabaseFixture _fixture;

        public MultiPickerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnCategoryFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.MatchingTransactions.First().Positions.First().Category;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions.Value
                             .Where(x => filterValue.MatchCategoryFilter(x.Category))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.CategoriesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.CategoriesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTagsFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = new [] { _fixture.ViewModelTests.Tags.Value[0], _fixture.ViewModelTests.Tags.Value[2] };
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions.Value
                             .Where(x => x.Tags.Any(y => filterValue.Contains(y)))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            var matchingSelectable = vm.State.TagsFilter.ComboBox.InternalDisplayableSearchResults
                                   .Where(x => filterValue.Any(y => y.Id == x.Id));
            foreach (var selectable in matchingSelectable)
                selectable.IsSelected = true;
            vm.State.TagsFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTypesFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.MatchingTransactions.First().Positions.First().Parent.Type;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions.Value
                             .Where(x => Equals(x.Parent.Type, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.TypesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.TypesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnUserStockFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.MatchingTransactions.First().Positions.First().Parent.UserStock;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelTests.Positions.Value
                             .Where(x => Equals(x.Parent.UserStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.UserStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.UserStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnExternalStockFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.MatchingTransactions.First().Positions.First().Parent.ExternalStock;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = vm.MatchingTransactions.SelectMany(x => x.Positions)
                             .Where(x => Equals(x.Parent.ExternalStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.ExternalStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.ExternalStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }
    }
}