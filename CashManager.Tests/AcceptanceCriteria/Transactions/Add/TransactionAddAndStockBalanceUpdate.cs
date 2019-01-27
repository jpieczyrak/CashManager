using System;

using Autofac;

using CashManager.Features.Main;
using CashManager.Features.Stocks;
using CashManager.Features.Transactions;
using CashManager.Tests.ViewModels.Fixtures;

using Xunit;

namespace CashManager.Tests.AcceptanceCriteria.Transactions.Add
{
    [Collection("Cleanable database collection")]
    public class TransactionAddAndStockBalanceUpdate : BaseTransactionTests
    {
        public TransactionAddAndStockBalanceUpdate(CleanableDatabaseFixture fixture) : base(fixture) { }

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
            app.SelectViewModelCommand.Execute(ViewModel.StockManager);
            var stocksViewModel = app.SelectedViewModel as StocksViewModel;
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
            Assert.Equal(expectedBalance, stocksViewModel.Stocks[0].UserBalance);
        }

        [Theory]
        [InlineData(1000, 500, -1, 0, 500)]
        [InlineData(1000, 500, 0, 0, 500)]
        [InlineData(1000, 500, 1, 0, 500)]
        [InlineData(1000, 500, -1, -5, 500)]
        [InlineData(1000, 500, 0, -5, 500)]
        [InlineData(1000, 500, 1, -5, 500)]
        [InlineData(1000, 500, -1, 5, 500)]
        [InlineData(1000, 500, 0, 5, 500)]
        [InlineData(1000, 500, 1, 5, 500)]
        public void AddingOutcomeTransaction_ModifyBalance_StockBalanceShouldBeModified(decimal startBalance, decimal transactionValue, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday, decimal expectedBalance)
        {
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);
            var type = CreateType(TransactionTypes.Outcome);
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.StockManager);
            var stocksViewModel = app.SelectedViewModel as StocksViewModel;
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
            Assert.Equal(expectedBalance, stocksViewModel.Stocks[0].UserBalance);
        }

        [Theory]
        [InlineData(1000, 500, -1, 0)]
        [InlineData(1000, 500, 0, 0)]
        [InlineData(1000, 500, 1, 0)]
        [InlineData(1000, 500, -1, -5)]
        [InlineData(1000, 500, 0, -5)]
        [InlineData(1000, 500, 1, -5)]
        [InlineData(1000, 500, -1, 5)]
        [InlineData(1000, 500, 0, 5)]
        [InlineData(1000, 500, 1, 5)]
        public void AddingIncomeTransaction_NoChange_StockBalanceShouldBeNotModified(decimal startBalance, decimal transactionValue, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday)
        {
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);
            var type = CreateType(TransactionTypes.Income);
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;

            transactionVm.SetUpdateMode(TransactionEditModes.NoChange);
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
            Assert.Equal(startBalance, _fixture.Container.Resolve<StocksViewModel>().Stocks[0].UserBalance);
        }
    }
}