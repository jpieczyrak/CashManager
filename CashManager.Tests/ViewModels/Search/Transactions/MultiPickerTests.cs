﻿using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Model;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class MultiPickerTests : ViewModelTests
    {
        [Fact]
        public void OnCategoryFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            Category filterValue = vm.Transactions.First().Positions.FirstOrDefault().Category;
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.Positions.Any(y => filterValue.MatchCategoryFilter(y.Category)))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.CategoriesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.SearchState.CategoriesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTagsFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            var filterValue = new[] { Tags[0], Tags[1] };
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.Positions.Any(y => y.Tags.Any(z => filterValue.Contains(z))))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            var matchingSelectable = vm.SearchState.TagsFilter.ComboBox.InternalDisplayableSearchResults
                                   .Where(x => filterValue.Any(y => y.Id == x.Id));
            foreach (var selectable in matchingSelectable)
                selectable.IsSelected = true;
            vm.SearchState.TagsFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnTypesFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            var filterValue = vm.Transactions.First().Type;
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => Equals(x.Type, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.TypesFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.SearchState.TypesFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnUserStockFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            var filterValue = vm.Transactions.First().UserStock;
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => Equals(x.UserStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.UserStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.SearchState.UserStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnExternalStockFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            var filterValue = vm.Transactions.First().ExternalStock;
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => Equals(x.ExternalStock, filterValue))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.SearchState.ExternalStocksFilter.ComboBox.InternalDisplayableSearchResults.First(x => x.Id == filterValue.Id).IsSelected = true;
            vm.SearchState.ExternalStocksFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }
    }
}