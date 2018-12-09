using System.Linq;

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
            Assert.Equal(DtoTransactions.Length, vm.Transactions.Length);
        }

        [Fact]
        public void OnPropertyChanged_SomeTransactionsTitleFilter_Filtered()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<TransactionSearchViewModel>();
            vm.Title.Value = DtoTransactions[0].Title;
            vm.Title.IsChecked = true;

            //when
            vm.RaisePropertyChanged(nameof(vm.Transactions));

            //then
            Assert.NotEmpty(vm.Transactions);
            var matching = DtoTransactions.Where(x => x.Title.ToLower().Contains(vm.Title.Value.ToLower())).ToArray();
            Assert.Equal(matching.Length, vm.Transactions.Length);
            Assert.Equal(matching.Select(x => x.Id), vm.Transactions.Select(x => x.Id));
        }
    }
}