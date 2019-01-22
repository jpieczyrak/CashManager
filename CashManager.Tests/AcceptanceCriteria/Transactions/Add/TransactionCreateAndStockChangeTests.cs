using System;

using Autofac;

using CashManager.Tests.ViewModels.Fixtures;

using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Stocks;
using CashManager_MVVM.Features.Transactions;

using Xunit;

namespace CashManager.Tests.AcceptanceCriteria.Transactions.Add
{
    [Collection("Cleanable database collection")]
    public class TransactionCreateAndStockChangeTests : BaseTransactionTests
    {
        public TransactionCreateAndStockChangeTests(CleanableDatabaseFixture fixture) : base(fixture) { }

        [Theory]
        [InlineData(1000, 500, TransactionTypes.Income, -1, 0, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, 0, 0, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, 1, 0, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, -1, -5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, 0, -5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, 1, -5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, -1, 5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, 0, 5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Income, 1, 5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, -1, 0, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, 0, 0, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, 1, 0, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, -1, -5, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, 0, -5, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, 1, -5, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, -1, 5, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, 0, 5, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome, 1, 5, 500)]
        public void AddingTransactionShouldModifyStockBalance(decimal startBalance, decimal transactionValue, TransactionTypes transactionType, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday, decimal expectedBalance)
        {
            //given
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);
            var type = CreateType(transactionType);
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel) app.SelectedViewModel;

            transactionVm.UpdateStock = true;
            transactionVm.Transaction.Title = "first one";
            transactionVm.Transaction.Type = type;
            transactionVm.Transaction.UserStock = userStock;
            transactionVm.Transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transactionVm.Transaction.Positions[0].Title = "title";
            transactionVm.Transaction.Positions[0].Value.GrossValue = transactionValue;

            //when
            transactionVm.SaveTransactionCommand.Execute(null);

            //then
            Assert.Single(transactionVm.TransactionsProvider.AllTransactions);
            Assert.Equal(expectedBalance, _fixture.Container.Resolve<StocksViewModel>().Stocks[0].UserBalance);
        }

        [Theory]
        [InlineData(1000, 500, -1, 0, 1500)]
        [InlineData(1000, 500, 0, 0, 1500)]
        [InlineData(1000, 500, 1, 0, 1500)]
        [InlineData(1000, 500, -1, -5, 1500)]
        [InlineData(1000, 500, 0, -5, 1500)]
        [InlineData(1000, 500, 1, -5, 1500)]
        [InlineData(1000, 500, -1, 5, 1500)]
        [InlineData(1000, 500, 0, 5, 1500)]
        [InlineData(1000, 500, 1, 5, 1500)]
        public void AddingIncomeTransaction_ModifyBalance_StockBalanceShouldBeModified(decimal startBalance, decimal transactionValue, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday, decimal expectedBalance)
        {
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);
            var type = CreateType(TransactionTypes.Income);
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;

            transactionVm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            transactionVm.Transaction.Title = "first one";
            transactionVm.Transaction.Type = type;
            transactionVm.Transaction.UserStock = userStock;
            transactionVm.Transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transactionVm.Transaction.Positions[0].Title = "title";
            transactionVm.Transaction.Positions[0].Value.GrossValue = transactionValue;

            //when
            transactionVm.SaveTransactionCommand.Execute(null);

            //then
            Assert.Single(transactionVm.TransactionsProvider.AllTransactions);
            Assert.Equal(expectedBalance, _fixture.Container.Resolve<StocksViewModel>().Stocks[0].UserBalance);
        }
    }
}