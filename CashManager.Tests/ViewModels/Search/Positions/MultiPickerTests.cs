using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
{
    public class MultiPickerTests : ViewModelTests
    {
        [Fact]
        public void OnCategoryFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.Transactions.First().Positions.First().Category;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => filterValue.MatchCategoryFilter(x.Category))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.CategoriesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.CategoriesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTagsFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = new [] { Tags[0], Tags[2] };
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
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
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTypesFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.Transactions.First().Positions.First().Parent.Type;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => Equals(x.Parent.Type, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.TypesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.TypesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnUserStockFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.Transactions.First().Positions.First().Parent.UserStock;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => Equals(x.Parent.UserStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.UserStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.UserStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnExternalStockFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.Transactions.First().Positions.First().Parent.ExternalStock;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = vm.Transactions.SelectMany(x => x.Positions)
                             .Where(x => Equals(x.Parent.ExternalStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.ExternalStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.ExternalStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }
    }
}