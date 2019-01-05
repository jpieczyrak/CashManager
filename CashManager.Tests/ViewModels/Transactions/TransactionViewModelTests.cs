using Autofac;

using CashManager_MVVM;
using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Model;

using Xunit;

namespace CashManager.Tests.ViewModels.Transactions
{
    public class TransactionViewModelTests : ViewModelTests
    {
        [Fact]
        public void SaveTransactionCommandExecute_ValidTransaction_TransactionIsBeingAddedToCommonState()
        {
            //given
            var vm = _container.Resolve<TransactionViewModel>();
            vm.Transaction = new Transaction
            {
                Title = "title 1",
                Positions = new TrulyObservableCollection<Position>(new [] { Positions[0] }),
                Type = Types[0],
                UserStock = Stocks[0]
            };
            vm.Transaction.Positions[0].TagViewModel = _container.Resolve<MultiComboBoxViewModel>();
            vm.Transaction.Positions[0].TagViewModel.SetInput(Tags);
            vm.ShouldGoBack = false;

            var command = vm.SaveTransactionCommand;

            //when
            bool canExecute = command.CanExecute(null);
            command.Execute(null);

            //then
            Assert.True(canExecute);
            Assert.Single(vm.TransactionsProvider.AllTransactions);
        }

        [Fact]
        public void SaveTransactionCommandExecute_ValidTransactionWhichAlreadyExists_TransactionIsBeingUpdated()
        {
            //given
            var vm = _container.Resolve<TransactionViewModel>();
            string title = "title 1";
            vm.Transaction = new Transaction
            {
                Title = title,
                Positions = new TrulyObservableCollection<Position>(new [] { Positions[0] }),
                Type = Types[0],
                UserStock = Stocks[0]
            };
            vm.Transaction.Positions[0].TagViewModel = _container.Resolve<MultiComboBoxViewModel>();
            vm.Transaction.Positions[0].TagViewModel.SetInput(Tags);
            vm.ShouldGoBack = false;

            var command = vm.SaveTransactionCommand;
            command.Execute(null);
            vm.Transaction.Title += 1;

            //when
            bool canExecute = command.CanExecute(null);
            command.Execute(null);

            //then
            Assert.True(canExecute);
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(vm.Transaction.Title, vm.TransactionsProvider.AllTransactions[0].Title);
        }
    }
}