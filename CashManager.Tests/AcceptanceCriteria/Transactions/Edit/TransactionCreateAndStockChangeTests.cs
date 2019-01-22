using System;
using System.Linq;

using Autofac;

using CashManager.Tests.ViewModels.Fixtures;

using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Features.Stocks;
using CashManager_MVVM.Features.Transactions;

using Xunit;

namespace CashManager.Tests.AcceptanceCriteria.Transactions.Edit
{
    [Collection("Cleanable database collection")]
    public class TransactionCreateAndStockChangeTests : BaseTransactionTests
    {
        public TransactionCreateAndStockChangeTests(CleanableDatabaseFixture fixture) : base(fixture) { }

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
            var sourceType = destinationType == originalType ? descType : CreateType(originalType);
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
                //todo: for some reason there is still search vm as "selected" instead of transaction vm...
                app.SelectViewModelCommand.Execute(ViewModel.Transaction);
                transactionVm = (TransactionViewModel)app.SelectedViewModel;
                transactionVm.Transaction = app.TransactionViewModel.Value.TransactionsProvider.AllTransactions.FirstOrDefault(x => x.Id == id);
                //todo: 3 lines above should be removed, when search test will be added and problem will be fixed.
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
    }
}