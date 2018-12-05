using Autofac;

using CashManager_MVVM.Features.Transactions;

using Xunit;

namespace CashManager.Tests.ViewModels
{
    public class TransactionSearchViewModelTests : ViewModelTests
    {
        [Fact]
        public void OnPropertyChanged_Clean_AllTransactions()
        {
            //given
            var vm = _container.Resolve<TransactionSearchViewModel>();

            //when
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.Empty(vm.Transactions);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactions_AllTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<TransactionSearchViewModel>();

            //when
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(Transactions.Length, vm.Transactions.Length);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactionsTitleFilter_Filtered()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<TransactionSearchViewModel>();
            vm.Title.Value = Transactions[0].Title;
            vm.Title.IsChecked = true;

            //when
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Single(vm.Transactions);
        }
    }
}