﻿using System.Linq;

using Autofac;

using CashManager.Features.Search;
using CashManager.Tests.ViewModels.Fixtures;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    [Collection("Database collection")]
    public class BaseTests
    {
        private readonly DatabaseFixture _fixture;

        public BaseTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnPropertyChanged_Clean_Null()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();

            //when
            vm.RaisePropertyChanged(nameof(vm.MatchingTransactions));

            //then
            Assert.Null(vm.MatchingTransactions);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactions_AllTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();

            //when
            vm.RaisePropertyChanged(nameof(vm.MatchingTransactions));

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(_fixture.ViewModelContext.Transactions.Value.Length, vm.MatchingTransactions.Count);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactionsTitleFilter_Filtered()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            vm.State.TitleFilter.Value = _fixture.ViewModelContext.Transactions.Value[0].Title;
            vm.State.TitleFilter.IsChecked = true;

            //when
            vm.RaisePropertyChanged(nameof(vm.MatchingTransactions));

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            var matching = _fixture.ViewModelContext.Transactions.Value.Where(x => x.Title.ToLower().Contains(vm.State.TitleFilter.Value.ToLower())).ToArray();
            Assert.Equal(matching.Length, vm.MatchingTransactions.Count);
            Assert.Equal(matching.Select(x => x.Id), vm.MatchingTransactions.Select(x => x.Id));
        }
    }
}