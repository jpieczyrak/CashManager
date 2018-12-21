using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class TransactionSearchWithFiltersTests : ViewModelTests
    {
        [Fact]
        public void OnPropertyChanged_Clean_AllTransactions()
        {
            //given
            var vm = _container.Resolve<SearchViewModel>();

            //when
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.Empty(vm.Transactions);
        }
    }
}