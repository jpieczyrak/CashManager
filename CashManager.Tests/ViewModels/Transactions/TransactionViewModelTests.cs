using System.Linq;

using Autofac;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Features.Common;
using CashManager.Features.Transactions;
using CashManager.Infrastructure.DbConnection;
using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Tests.ViewModels.Fixtures;

using LiteDB;

using Xunit;

namespace CashManager.Tests.ViewModels.Transactions
{
    [Collection("Cleanable database collection")]
    public class TransactionViewModelTests
    {
        private readonly Tag[] _tags = { new Tag { Name = "1" }, new Tag { Name = "2" } };

        private CashManager.Data.DTO.Tag[] DtoTags => Mapper.Map<CashManager.Data.DTO.Tag[]>(_tags);

        private readonly Category[] _categories =
        {
            new Category { Name = "1" },
            new Category { Name = "2" },
            new Category { Name = "3" }
        };

        private CashManager.Data.DTO.Category[] DtoCategories => Mapper.Map<CashManager.Data.DTO.Category[]>(_categories);

        private readonly CleanableDatabaseFixture _fixture;

        public TransactionViewModelTests(CleanableDatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.Container.Resolve<TransactionsProvider>().AllTransactions.Clear();
            _fixture.Reset();
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
            vm.Transaction.Positions[0].TagViewModel.SetInput(_tags.Select(x => new Selectable(x)));
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
            _fixture.Container.Resolve<LiteRepository>().Database.UpsertBulk(DtoTags);
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            vm.Update();
            string title = "title 1";
            vm.Transaction = new Transaction
            {
                Title = title,
                Positions = new TrulyObservableCollection<Position>(new [] { new Position() }),
                Type = new TransactionType(),
                UserStock = new Stock()
            };
            vm.AddNewPosition.Execute(null);
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
            vm.Transaction.Positions[0].TagViewModel.SetInput(_tags.Select(x => new Selectable(x)));
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
            vm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            vm.Transaction.Positions[0].TagViewModel = _fixture.Container.Resolve<MultiComboBoxViewModel>();
            vm.Transaction.Positions[0].TagViewModel.SetInput(_tags.Select(x => new Selectable(x)));
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

        [Fact]
        public void CancelTransactionCommand_EditedTransaction_TransactionIsNotEdited()
        {
            //given
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            vm.ShouldGoBack = false;
            string startTitle = "t";
            var transaction = new Transaction
            {
                Title = startTitle,
                UserStock = new Stock(),
                Type = new TransactionType(),
                Positions = new TrulyObservableCollection<Position>(new[] { new Position() })
            };

            //transaction exists
            vm.Transaction = transaction;
            vm.Update();
            vm.SaveTransactionCommand.Execute(null);

            vm.Transaction = transaction;
            vm.Update();
            vm.Transaction.Title += "edited";

            //when
            vm.CancelTransactionCommand.Execute(null);

            //then
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(startTitle, vm.TransactionsProvider.AllTransactions[0].Title);
        }

        [Fact]
        public void TransactionCreation_NoTags_NoExceptions()
        {
            //given
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            vm.Update(); //creates transactions ect (anyway should be perform on view show)

            //when
            vm.AddNewPosition.Execute(null);

            //then
            Assert.NotEmpty(vm.Transaction.Positions);
        }

        [Fact]
        public void TransactionCreation_TwoPositions_TagsAreNotBeingSynchronizedBetweenPositions()
        {
            //given
            _fixture.Container.Resolve<LiteRepository>().Database.UpsertBulk(DtoTags);
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            vm.ShouldGoBack = false;
            vm.Update(); //creates transactions ect (anyway should be perform on view show)

            vm.AddNewPosition.Execute(null);

            //when
            vm.Transaction.Positions[0].TagViewModel.InternalDisplayableSearchResults[0].IsSelected = true;

            //then
            Assert.True(vm.Transaction.Positions[0].TagViewModel.InternalDisplayableSearchResults[0].IsSelected);
            Assert.True(vm.Transaction.Positions[1].TagViewModel.InternalDisplayableSearchResults.All(x => x.IsSelected == false));
        }

        [Fact]
        public void TransactionCreation_TwoPositions_CategoriesAreNotBeingSynchronizedBetweenPositions()
        {
            //given
            _fixture.Container.Resolve<LiteRepository>().Database.UpsertBulk(DtoCategories);
            var vm = _fixture.Container.Resolve<TransactionViewModel>();
            vm.ShouldGoBack = false;
            vm.Update(); //creates transactions ect (anyway should be perform on view show)

            vm.AddNewPosition.Execute(null);

            //when
            vm.Transaction.Positions[0].CategoryPickerViewModel.SelectedCategory = _categories[1];
            vm.Transaction.Positions[1].CategoryPickerViewModel.SelectedCategory = _categories[2];

            //then
            Assert.NotSame(vm.Transaction.Positions[0].CategoryPickerViewModel.SelectedCategory, vm.Transaction.Positions[1].CategoryPickerViewModel.SelectedCategory);
            Assert.NotSame(vm.Transaction.Positions[0].Category, vm.Transaction.Positions[1].Category);
        }
    }
}