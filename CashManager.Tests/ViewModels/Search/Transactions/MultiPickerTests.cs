using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Model;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
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
        public void OnCategoryFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            Category filterValue = vm.MatchingTransactions.First().Positions.FirstOrDefault().Category;
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.Positions.Any(y => filterValue.MatchCategoryFilter(y.Category)))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.CategoriesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.CategoriesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTagsFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = new[] { _fixture.ViewModelTests.Tags[0], _fixture.ViewModelTests.Tags[1] };
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.Positions.Any(y => y.Tags.Any(z => filterValue.Contains(z))))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            var matchingSelectable = vm.State.TagsFilter.ComboBox.InternalDisplayableSearchResults
                                   .Where(x => filterValue.Any(y => y.Id == x.Id));
            foreach (var selectable in matchingSelectable)
                selectable.IsSelected = true;
            vm.State.TagsFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTypesFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.MatchingTransactions.First().Type;
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => Equals(x.Type, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.TypesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.TypesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnUserStockFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.MatchingTransactions.First().UserStock;
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => Equals(x.UserStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.UserStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.UserStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnExternalStockFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            var filterValue = vm.MatchingTransactions.First().ExternalStock;
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => Equals(x.ExternalStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.ExternalStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.State.ExternalStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }
    }
}