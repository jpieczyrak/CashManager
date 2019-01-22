using System;

using Autofac;

using CashManager.Tests.ViewModels.Fixtures;

using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Features.Stocks;
using CashManager_MVVM.Features.Transactions;
using CashManager_MVVM.Features.TransactionTypes;
using CashManager_MVVM.Model;

using Xunit;

namespace CashManager.Tests.AcceptanceCriteria.Transactions.Edit
{
    [Collection("Cleanable database collection")]
    public class TransactionCreateAndStockChangeTests : BaseTransactionTests
    {
        public TransactionCreateAndStockChangeTests(CleanableDatabaseFixture fixture) : base(fixture) { }

        [Theory]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, -1, 0, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 0, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 1, 0, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, -1, -5, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, -5, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 1, -5, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, -1, 5, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 0, 5, 500)]
        [InlineData(1000, 500, TransactionTypes.Income, TransactionTypes.Outcome, 1, 5, 500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, -1, 0, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, 0, 0, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, 1, 0, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, -1, -5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, 0, -5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, 1, -5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, -1, 5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, 0, 5, 1500)]
        [InlineData(1000, 500, TransactionTypes.Outcome,TransactionTypes.Income, 1, 5, 1500)]
        public void EditingTransactionShouldModifyStockBalance(decimal startBalance, decimal transactionValue, TransactionTypes originalType, TransactionTypes destinationType, int daysSinceLastStockEdit, int transactionBookDateAsDaysCountUntilToday, decimal expectedBalance)
        {
            //given
            var app = _fixture.Container.Resolve<ApplicationViewModel>();

            var descType = CreateType(destinationType);
            var sourceType = CreateType(originalType);
            var userStock = CreateUserStock(startBalance, daysSinceLastStockEdit);

            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel) app.SelectedViewModel;

            transactionVm.UpdateStock = true;
            var transaction = transactionVm.Transaction;
            transaction.Title = "first one unedited";
            transaction.Type = sourceType;
            transaction.UserStock = userStock;
            transaction.BookDate = DateTime.Today.AddDays(-transactionBookDateAsDaysCountUntilToday);
            transaction.Positions[0].Title = "title";
            transaction.Positions[0].Value.GrossValue = transactionValue;
            transactionVm.SaveTransactionCommand.Execute(null);

            app.SelectViewModelCommand.Execute(ViewModel.Search);
            var searchVm = (SearchViewModel) app.SelectedViewModel;
            searchVm.TransactionsListViewModel.SelectedTransaction = transaction;
            Assert.True(searchVm.TransactionsListViewModel.TransactionEditCommand.CanExecute(null));
            searchVm.TransactionsListViewModel.TransactionEditCommand.Execute(null);

            app.TransactionViewModel.Value.Transaction.Type = descType;

            //when
            //todo: check if need: for some reason there is still search vm as "selected" instead of transaction vm...
            app.TransactionViewModel.Value.SaveTransactionCommand.Execute(null);

            //then
            Assert.Equal(2, app.TransactionViewModel.Value.TransactionsProvider.AllTransactions.Count);
            Assert.Equal(2, searchVm.TransactionsListViewModel.Transactions.Count);
            Assert.Equal(expectedBalance, _fixture.Container.Resolve<StocksViewModel>().Stocks[0].UserBalance);
        }

        private TransactionType CreateType(TransactionTypes type)
        {
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.TypesManager);
            var vm = (TransactionTypesViewModel) app.SelectedViewModel;
            vm.AddTransactionTypeCommand.Execute(null);
            var transactionType = vm.TransactionTypes[0];

            switch (type)
            {
                case TransactionTypes.Income:
                    transactionType.Income = true;
                    transactionType.Name = "income";
                    break;
                case TransactionTypes.Outcome:
                    transactionType.Outcome = true;
                    transactionType.Name = "outcome";
                    break;
            }

            return transactionType;
        }

        private Stock CreateUserStock(decimal balanceValue, int daysSinceLastStockEdit)
        {
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.StockManager);
            var stockVm = (StocksViewModel) app.SelectedViewModel;
            stockVm.AddStockCommand.Execute(null);
            var userStock = stockVm.Stocks[0];
            userStock.IsUserStock = true;
            userStock.Name = "User stock no 1";
            userStock.Balance.Value = balanceValue;

            if (daysSinceLastStockEdit != 0)
            {
                userStock.Balance
                         .GetType()
                         .GetProperty("LastEditDate")?
                         .SetValue(userStock.Balance, DateTime.Today.AddDays(-daysSinceLastStockEdit));
            }

            return userStock;
        }
    }
}