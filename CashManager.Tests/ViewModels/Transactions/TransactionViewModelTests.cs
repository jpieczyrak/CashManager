using Autofac;

using CashManager.Tests.ViewModels.Fixtures;

using CashManager_MVVM;
using CashManager_MVVM.CommonData;
using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Model;

using Xunit;

namespace CashManager.Tests.ViewModels.Transactions
{
    [Collection("Cleanable database collection")]
    public class TransactionViewModelTests
    {
        private readonly Tag[] _tags = { new Tag(), new Tag() };
        private readonly CleanableDatabaseFixture _fixture;

        public TransactionViewModelTests(CleanableDatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.Container.Resolve<TransactionsProvider>().AllTransactions.Clear();
            _fixture.CleanDatabase();
        }

        [Fact]
        public void SaveTransactionCommandExecute_ValidTransaction_TransactionIsBeingAddedToCommonState()
        {
            //given
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            vm.Transaction = new Transaction
            {
                Title = "title 1",
                Positions = new TrulyObservableCollection<Position>(new [] { new Position() }),
                Type = new TransactionType(),
                UserStock = new Stock()
            };
            vm.Transaction.Positions[0].TagViewModel = _fixture.Container.Resolve<MultiComboBoxViewModel>();
            vm.Transaction.Positions[0].TagViewModel.SetInput(_tags);
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
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            string title = "title 1";
            vm.Transaction = new Transaction
            {
                Title = title,
                Positions = new TrulyObservableCollection<Position>(new [] { new Position() }),
                Type = new TransactionType(),
                UserStock = new Stock()
            };
            vm.Transaction.Positions[0].TagViewModel = _fixture.Container.Resolve<MultiComboBoxViewModel>();
            vm.Transaction.Positions[0].TagViewModel.SetInput(_tags);
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


        [Theory]
        [InlineData(true, 0, 200, 200)]
        [InlineData(true, 100, 200, 300)]
        [InlineData(false, 200, 100, 100)]
        [InlineData(false, 0, 200, -200)]
        public void StockBalanceUpdate_AddValidTransaction_StockBalanceUpdated(bool income, decimal startBalance, decimal transValue, decimal expectedBalance)
        {
            //given
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            vm.Update();
            var userStock = new Stock { Name = "test", Balance = new Balance { Value = startBalance }, IsUserStock = true };

            vm.Transaction.Positions = new TrulyObservableCollection<Position>(new[] {
                new Position
                {
                    Title = "test1", Value = new PaymentValue(transValue, transValue, 0m)
                }
            });
            vm.Transaction.Title = "non empty";
            vm.Transaction.Type = new TransactionType { Income = income, Outcome = !income };
            vm.Transaction.UserStock = userStock;
            vm.Transaction.Positions[0].TagViewModel = _fixture.Container.Resolve<MultiComboBoxViewModel>();
            vm.Transaction.Positions[0].TagViewModel.SetInput(_tags);
            vm.ShouldGoBack = false;

            var command = vm.SaveTransactionCommand;

            //when
            bool canExecute = command.CanExecute(null);
            command.Execute(null);

            //then
            Assert.True(canExecute);
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(expectedBalance, userStock.Balance.Value);
        }

        [Theory]
        [InlineData(true, 1000, 1000, 1500, 1500)]
        [InlineData(true, 1000, 1000, 500, 500)]
        [InlineData(true, 1000, 500, 100, -400)]
        [InlineData(false, 1000, 1000, 100, 1900)]
        [InlineData(false, 1500, 500, 1500, 500)]
        [InlineData(false, 500, 500, 1500, -500)]
        public void StockBalanceUpdate_EditValidTransaction_StockBalanceUpdated(bool income, decimal transactionStartValue, decimal startBalance, decimal transValue, decimal expectedBalance)
        {
            //given
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            var userStock = new Stock { Name = "test", Balance = new Balance { Value = startBalance }, IsUserStock = true };
            //assigning transaction = transaction edit
            vm.Transaction = new Transaction
            {
                Title = "non empty",
                Positions = new TrulyObservableCollection<Position>(new[] {
                new Position
                {
                    Title = "test1", Value = new PaymentValue(transactionStartValue, transactionStartValue, 0m)
                }
                }),
                Type = new TransactionType { Income = income, Outcome = !income},
                UserStock = userStock
            };
            vm.Transaction.Positions[0].TagViewModel = _fixture.Container.Resolve<MultiComboBoxViewModel>();
            vm.Transaction.Positions[0].TagViewModel.SetInput(_tags);
            vm.Transaction.Positions[0].Value.GrossValue = transValue;
            vm.Transaction.Positions[0].Value.NetValue = transValue;
            vm.ShouldGoBack = false;

            var command = vm.SaveTransactionCommand;

            //when
            bool canExecute = command.CanExecute(null);
            command.Execute(null);

            //then
            Assert.True(canExecute);
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(expectedBalance, userStock.Balance.Value);
        }
    }
}