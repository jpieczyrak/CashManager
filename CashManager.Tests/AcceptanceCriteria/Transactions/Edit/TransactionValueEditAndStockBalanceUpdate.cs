using System;
using System.Linq;

using Autofac;

using CashManager.Features.Main;
using CashManager.Features.Search;
using CashManager.Features.Stocks;
using CashManager.Features.Transactions;
using CashManager.Tests.ViewModels.Fixtures;

using Xunit;

namespace CashManager.Tests.AcceptanceCriteria.Transactions.Edit
{
    [Collection("Cleanable database collection")]
    public class TransactionValueEditAndStockBalanceUpdate : BaseTransactionTests
    {
        public TransactionValueEditAndStockBalanceUpdate(CleanableDatabaseFixture fixture) : base(fixture) { }

        [Theory]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, -1, 0, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 0, 0, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 1, 0, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, -1, -5, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 0, -5, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 1, -5, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, -1, 5, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 0, 5, 1500)]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 1, 5, 1500)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, -1, 0, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, 0, 0, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, 1, 0, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, -1, -5, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, 0, -5, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, 1, -5, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, -1, 5, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, 0, 5, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Income, 1, 5, 2000)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, -1, 0, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 0, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 1, 0, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, -1, -5, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, -5, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 1, -5, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, -1, 5, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 5, 500)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 1, 5, 500)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, -1, 0, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, 0, 0, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, 1, 0, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, -1, -5, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, 0, -5, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, 1, -5, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, -1, 5, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, 0, 5, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Outcome, 1, 5, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Income, 0, 0, 2000)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, 0, 0)]
        public void EditTransaction_ModifyStock_StockBalanceShouldBeModified(decimal startBalance, decimal transactionValue, decimal newValue, TransactionTypes originalType, TransactionTypes destinationType, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday, decimal expectedBalance)
        {
            //given
            var app = _fixture.Container.Resolve<ApplicationViewModel>();

            var descType = CreateType(destinationType);
            var sourceType = CreateType(originalType);
            if (destinationType == originalType && destinationType == TransactionTypes.Outcome)
                CreateType(TransactionTypes.Income); //there have to be income type to create stock balance
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);

            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;

            transactionVm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            var transaction = transactionVm.Transaction;
            var id = transaction.Id;
            transaction.Title = "first one unedited";
            transaction.Type = sourceType;
            transaction.UserStock = userStock;
            transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transaction.Positions[0].Title = "title";
            transaction.Positions[0].Value.GrossValue = transactionValue;
            transactionVm.SaveTransactionCommand.Execute(null);

            app.SelectViewModelCommand.Execute(ViewModel.Search);
            var searchVm = (SearchViewModel)app.SelectedViewModel;

            //edit
            searchVm.TransactionsListViewModel.SelectedTransaction = searchVm.TransactionsListViewModel.Transactions.FirstOrDefault(x => x.Id == id);
            searchVm.TransactionsListViewModel.TransactionEditCommand.Execute(null);

            //when
            transactionVm.Transaction.Type = descType;
            transactionVm.Transaction.Positions[0].Value.GrossValue = newValue;
            Assert.True(transactionVm.IsInEditMode);
            transactionVm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            transactionVm.SaveTransactionCommand.Execute(null);

            //then
            Assert.Equal(2, app.TransactionViewModel.Value.TransactionsProvider.AllTransactions.Count); //one is from stock creation
            Assert.Equal(2, searchVm.TransactionsListViewModel.Transactions.Count);
            var stocksViewModel = _fixture.Container.Resolve<StocksViewModel>();
            Assert.Equal(expectedBalance, stocksViewModel.Stocks[0].UserBalance);
        }

        [Theory]
        [InlineData(1000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 0, 0)]
        [InlineData(1000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Income, TransactionTypes.Income, 0, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, -1, 0)]
        [InlineData(1000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, -1)]
        public void EditTransaction_NoChange_StockBalanceShouldNotBeModified(decimal startBalance, decimal transactionValue, decimal newValue, TransactionTypes originalType, TransactionTypes destinationType, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday)
        {
            //given
            var app = _fixture.Container.Resolve<ApplicationViewModel>();

            var descType = CreateType(destinationType);
            var sourceType = CreateType(originalType);
            if (destinationType == originalType && destinationType == TransactionTypes.Outcome)
                CreateType(TransactionTypes.Income); //there have to be income type to create stock balance
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);

            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;

            transactionVm.SetUpdateMode(TransactionEditModes.NoChange);
            var transaction = transactionVm.Transaction;
            var id = transaction.Id;
            transaction.Title = "first one unedited";
            transaction.Type = sourceType;
            transaction.UserStock = userStock;
            transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transaction.Positions[0].Title = "title";
            transaction.Positions[0].Value.GrossValue = transactionValue;
            transactionVm.SaveTransactionCommand.Execute(null);

            app.SelectViewModelCommand.Execute(ViewModel.Search);
            var searchVm = (SearchViewModel)app.SelectedViewModel;

            //edit
            searchVm.TransactionsListViewModel.SelectedTransaction = searchVm.TransactionsListViewModel.Transactions.FirstOrDefault(x => x.Id == id);
            searchVm.TransactionsListViewModel.TransactionEditCommand.Execute(null);

            //when
            transactionVm.Transaction.Type = descType;
            transactionVm.Transaction.Positions[0].Value.GrossValue = newValue;
            Assert.True(transactionVm.IsInEditMode);
            transactionVm.SetUpdateMode(TransactionEditModes.NoChange);
            transactionVm.SaveTransactionCommand.Execute(null);

            //then
            Assert.Equal(2, app.TransactionViewModel.Value.TransactionsProvider.AllTransactions.Count); //one is from stock creation
            Assert.Equal(2, searchVm.TransactionsListViewModel.Transactions.Count);
            var stocksViewModel = _fixture.Container.Resolve<StocksViewModel>();
            Assert.Equal(startBalance, stocksViewModel.Stocks[0].UserBalance);
        }

        [Theory]
        [InlineData(10000, 500, 500, TransactionTypes.Outcome, TransactionTypes.Income, 0, 0)]
        [InlineData(10000, 500, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 0)]
        [InlineData(10000, 500, 1000, TransactionTypes.Income, TransactionTypes.Income, 0, 0)]
        [InlineData(10000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, 0)]
        [InlineData(10000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, -1, 0)]
        [InlineData(10000, 500, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, -1)]
        public void EditTransaction_AddCorrection_StockBalanceShouldNotBeModifiedButThereShouldBeCorrectionTransaction(decimal startBalance, decimal transactionValue, decimal newValue, TransactionTypes originalType, TransactionTypes destinationType, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday)
        {
            //given
            var app = _fixture.Container.Resolve<ApplicationViewModel>();

            decimal transactionValueAsProfit = (originalType == TransactionTypes.Income ? transactionValue : -transactionValue);
            decimal diff = transactionValueAsProfit - (destinationType == TransactionTypes.Income ? newValue : -newValue);
            var descType = CreateType(destinationType);
            var sourceType = CreateType(originalType);

            //there have to be income type to create stock balance
            if (destinationType == originalType && destinationType == TransactionTypes.Outcome) CreateType(TransactionTypes.Income);
            //there have to be outcome type to create stock balance
            if (destinationType == originalType && destinationType == TransactionTypes.Income) CreateType(TransactionTypes.Outcome);
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);

            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;

            transactionVm.SetUpdateMode(TransactionEditModes.NoChange); //it is creation, just leave not change here
            var transaction = transactionVm.Transaction;
            var id = transaction.Id;
            transaction.Title = "first one unedited";
            transaction.Type = sourceType;
            transaction.UserStock = userStock;
            transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transaction.Positions[0].Title = "title";
            transaction.Positions[0].Value.GrossValue = transactionValue;
            transactionVm.SaveTransactionCommand.Execute(null);

            app.SelectViewModelCommand.Execute(ViewModel.Search);
            var searchVm = (SearchViewModel)app.SelectedViewModel;

            //edit
            searchVm.TransactionsListViewModel.SelectedTransaction = searchVm.TransactionsListViewModel.Transactions.FirstOrDefault(x => x.Id == id);
            searchVm.TransactionsListViewModel.TransactionEditCommand.Execute(null);

            //when
            transactionVm.Transaction.Type = descType;
            transactionVm.Transaction.Positions[0].Value.GrossValue = newValue;
            Assert.True(transactionVm.IsInEditMode);
            transactionVm.SetUpdateMode(TransactionEditModes.AddCorrection);
            transactionVm.SaveTransactionCommand.Execute(null);

            //then
            var transactions = app.TransactionViewModel.Value.TransactionsProvider.AllTransactions;
            Assert.Equal(3, transactions.Count);
            Assert.Equal(3, searchVm.TransactionsListViewModel.Transactions.Count);
            var stocksViewModel = _fixture.Container.Resolve<StocksViewModel>();
            Assert.Equal(startBalance, stocksViewModel.Stocks[0].UserBalance);
            Assert.Equal(transactionValueAsProfit, transactions[1].ValueAsProfit + transactions[2].ValueAsProfit);
            Assert.Equal(diff, transactions.Last().ValueAsProfit);
        }

        [Theory]
        [InlineData(5000, 1000, 500, TransactionTypes.Outcome, TransactionTypes.Income, 0, 0)]
        [InlineData(5000, 1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Income, TransactionTypes.Income, 0, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, -1, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, -1)]
        public void EditTransaction_UpdateBalanceAnotherStock_StocksBalancesShouldBeModified(decimal startBalance, decimal transactionValue, decimal newValue, TransactionTypes originalType, TransactionTypes destinationType, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday)
        {
            //given
            var app = _fixture.Container.Resolve<ApplicationViewModel>();

            var descType = CreateType(destinationType);
            var sourceType = CreateType(originalType);

            //there have to be income type to create stock balance
            if (destinationType == originalType && destinationType == TransactionTypes.Outcome) CreateType(TransactionTypes.Income);
            //there have to be outcome type to create stock balance
            if (destinationType == originalType && destinationType == TransactionTypes.Income) CreateType(TransactionTypes.Outcome);
            var firstUserStock = CreateUserStock(startBalance, daysSinceLastStockEdit);
            var secondUserStock = CreateUserStock(startBalance, daysSinceLastStockEdit);

            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;

            transactionVm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            var transaction = transactionVm.Transaction;
            var id = transaction.Id;
            transaction.Title = "first one unedited";
            transaction.Type = sourceType;
            transaction.UserStock = firstUserStock;
            transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transaction.Positions[0].Title = "title";
            transaction.Positions[0].Value.GrossValue = transactionValue;
            transactionVm.SaveTransactionCommand.Execute(null);

            app.SelectViewModelCommand.Execute(ViewModel.Search);
            var searchVm = (SearchViewModel)app.SelectedViewModel;

            //edit
            searchVm.TransactionsListViewModel.SelectedTransaction = searchVm.TransactionsListViewModel.Transactions.FirstOrDefault(x => x.Id == id);
            searchVm.TransactionsListViewModel.TransactionEditCommand.Execute(null);

            //when
            transactionVm.Transaction.Type = descType;
            transactionVm.Transaction.UserStock = secondUserStock;
            transactionVm.Transaction.Positions[0].Value.GrossValue = newValue;
            Assert.True(transactionVm.IsInEditMode);
            transactionVm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            transactionVm.SaveTransactionCommand.Execute(null);

            //then
            var transactions = app.TransactionViewModel.Value.TransactionsProvider.AllTransactions;
            Assert.Equal(3, transactions.Count);
            Assert.Equal(3, searchVm.TransactionsListViewModel.Transactions.Count);

            app.SelectViewModelCommand.Execute(ViewModel.StockManager);
            var stocksViewModel = app.SelectedViewModel as StocksViewModel;

            Assert.Equal(startBalance, stocksViewModel.Stocks[0].UserBalance);
            decimal expectedSecondStockBalance = startBalance + (destinationType == TransactionTypes.Income ? newValue : -newValue);
            Assert.Equal(expectedSecondStockBalance, secondUserStock.UserBalance);
            Assert.Equal(expectedSecondStockBalance, stocksViewModel.Stocks[1].UserBalance);
        }

        [Theory]
        [InlineData(5000, 1000, 500, TransactionTypes.Outcome, TransactionTypes.Income, 0, 0)]
        [InlineData(5000, 1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Income, TransactionTypes.Income, 0, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, -1, 0)]
        [InlineData(5000, 1000, 1000, TransactionTypes.Outcome, TransactionTypes.Outcome, 0, -1)]
        public void EditTransaction_AddCorrectionAnotherStock_StocksBalancesShouldBeModified(decimal startBalance, decimal transactionValue, decimal newValue, TransactionTypes originalType, TransactionTypes destinationType, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday)
        {
            //given
            var app = _fixture.Container.Resolve<ApplicationViewModel>();

            var descType = CreateType(destinationType);
            var sourceType = CreateType(originalType);

            //there have to be income type to create stock balance
            if (destinationType == originalType && destinationType == TransactionTypes.Outcome) CreateType(TransactionTypes.Income);
            //there have to be outcome type to create stock balance
            if (destinationType == originalType && destinationType == TransactionTypes.Income) CreateType(TransactionTypes.Outcome);
            var firstUserStock = CreateUserStock(startBalance, daysSinceLastStockEdit);
            var secondUserStock = CreateUserStock(startBalance, daysSinceLastStockEdit);

            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;

            transactionVm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            var transaction = transactionVm.Transaction;
            var id = transaction.Id;
            transaction.Title = "first one unedited";
            transaction.Type = sourceType;
            transaction.UserStock = firstUserStock;
            transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transaction.Positions[0].Title = "title";
            transaction.Positions[0].Value.GrossValue = transactionValue;
            transactionVm.SaveTransactionCommand.Execute(null);

            app.SelectViewModelCommand.Execute(ViewModel.Search);
            var searchVm = (SearchViewModel)app.SelectedViewModel;

            //edit
            searchVm.TransactionsListViewModel.SelectedTransaction = searchVm.TransactionsListViewModel.Transactions.FirstOrDefault(x => x.Id == id);
            searchVm.TransactionsListViewModel.TransactionEditCommand.Execute(null);

            //when
            transactionVm.Transaction.Type = descType;
            transactionVm.Transaction.UserStock = secondUserStock;
            transactionVm.Transaction.Positions[0].Value.GrossValue = newValue;
            Assert.True(transactionVm.IsInEditMode);
            transactionVm.SetUpdateMode(TransactionEditModes.AddCorrection);
            transactionVm.SaveTransactionCommand.Execute(null);

            //then
            var transactions = app.TransactionViewModel.Value.TransactionsProvider.AllTransactions;
            Assert.Equal(4, transactions.Count);
            Assert.Equal(4, searchVm.TransactionsListViewModel.Transactions.Count);

            app.SelectViewModelCommand.Execute(ViewModel.StockManager);
            var stocksViewModel = app.SelectedViewModel as StocksViewModel;

            decimal firstTransactionValue = (sourceType.Income ? transactionValue : -transactionValue);
            decimal expectedFirstStockBalance = startBalance + firstTransactionValue;
            Assert.Equal(expectedFirstStockBalance, stocksViewModel.Stocks[0].UserBalance);
            decimal expectedSecondStockBalance = startBalance + (destinationType == TransactionTypes.Income ? newValue : -newValue);
            Assert.Equal(expectedSecondStockBalance, secondUserStock.UserBalance);
            Assert.Equal(expectedSecondStockBalance, stocksViewModel.Stocks[1].UserBalance);

            Assert.Equal(firstTransactionValue, transactions.Last().ValueAsProfit);
        }
    }
}