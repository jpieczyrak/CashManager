using System;
using System.Linq;

using Autofac;

using CashManager.Tests.AcceptanceCriteria.Transactions;
using CashManager.Tests.ViewModels.Fixtures;

using CashManager_MVVM.Features;
using CashManager_MVVM.Features.Main;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Features.Transactions;

using Xunit;

namespace CashManager.Tests.AcceptanceCriteria.Search
{
    [Collection("Cleanable database collection")]
    public class NavigationTests : BaseTransactionTests
    {
        public NavigationTests(CleanableDatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public void NavigationShouldWorkProperly_ThereIsNoBetterName()
        {
            //given
            var app = _fixture.Container.Resolve<ApplicationViewModel>();

            var type = CreateType(TransactionTypes.Income);
            var userStock = CreateUserStock(1000, 0);

            //when
            app.SelectViewModelCommand.Execute(ViewModel.Transaction);
            var transactionVm = (TransactionViewModel)app.SelectedViewModel;
            Assert.NotNull(transactionVm);

            transactionVm.SetUpdateMode(TransactionEditModes.ChangeStockBalance);
            var transaction = transactionVm.Transaction;
            transaction.Title = "title";
            transaction.Type = type;
            transaction.UserStock = userStock;
            transaction.BookDate = DateTime.Today;
            transactionVm.SaveTransactionCommand.Execute(null);

            app.SelectViewModelCommand.Execute(ViewModel.Search);
            var searchVm = (SearchViewModel)app.SelectedViewModel;
            Assert.NotNull(searchVm);
            Assert.Equal(2, searchVm.TransactionsListViewModel.Transactions.Count);

            var selectedTransaction = searchVm.TransactionsListViewModel.Transactions.First();
            searchVm.TransactionsListViewModel.SelectedTransaction = selectedTransaction;
            searchVm.TransactionsListViewModel.TransactionEditCommand.Execute(null);
            Assert.True(app.SelectedViewModel is TransactionViewModel);

            transactionVm = (TransactionViewModel)app.SelectedViewModel;
            Assert.NotNull(transactionVm);
            Assert.Equal(selectedTransaction, transactionVm.Transaction);
            Assert.True(transactionVm.IsInEditMode);

            transactionVm.SaveTransactionCommand.Execute(null);
            Assert.True(app.SelectedViewModel is SearchViewModel);

            Assert.Equal(2, app.TransactionViewModel.Value.TransactionsProvider.AllTransactions.Count);
            Assert.Equal(2, searchVm.TransactionsListViewModel.Transactions.Count);
        }
    }
}