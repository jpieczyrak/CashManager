using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class BaseTests : ViewModelTests
    {
        [Fact]
        public void OnPropertyChanged_Clean_Null()
        {
            //given
            var vm = _container.Resolve<SearchViewModel>();

            //when
            vm.RaisePropertyChanged(nameof(vm.MatchingTransactions));

            //then
            Assert.Null(vm.MatchingTransactions);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactions_AllTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();

            //when
            vm.RaisePropertyChanged(nameof(vm.MatchingTransactions));

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(Transactions.Length, vm.MatchingTransactions.Count);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactionsTitleFilter_Filtered()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();
            vm.State.TitleFilter.Value = Transactions[0].Title;
            vm.State.TitleFilter.IsChecked = true;

            //when
            vm.RaisePropertyChanged(nameof(vm.MatchingTransactions));

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            var matching = Transactions.Where(x => x.Title.ToLower().Contains(vm.State.TitleFilter.Value.ToLower())).ToArray();
            Assert.Equal(matching.Length, vm.MatchingTransactions.Count);
            Assert.Equal(matching.Select(x => x.Id), vm.MatchingTransactions.Select(x => x.Id));
        }
    }
}