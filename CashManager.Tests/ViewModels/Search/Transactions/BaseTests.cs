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
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.Null(vm.Transactions);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactions_AllTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            vm.Update();

            //when
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(Transactions.Length, vm.Transactions.Count);
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
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.NotEmpty(vm.Transactions);
            var matching = Transactions.Where(x => x.Title.ToLower().Contains(vm.State.TitleFilter.Value.ToLower())).ToArray();
            Assert.Equal(matching.Length, vm.Transactions.Count);
            Assert.Equal(matching.Select(x => x.Id), vm.Transactions.Select(x => x.Id));
        }
    }
}