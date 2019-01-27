using System;
using System.Linq;

using Autofac;

using CashManager.Tests.ViewModels.Fixtures;

using CashManager.WPF.Features.Main;
using CashManager.WPF.Features.Stocks;
using CashManager.WPF.Features.TransactionTypes;
using CashManager.WPF.Model;

namespace CashManager.Tests.AcceptanceCriteria.Transactions
{
    public abstract class BaseTransactionTests
    {
        protected readonly CleanableDatabaseFixture _fixture;

        protected BaseTransactionTests(CleanableDatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.Reset();
        }

        protected TransactionType CreateType(TransactionTypes type)
        {
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.TypesManager);
            var vm = (TransactionTypesViewModel)app.SelectedViewModel;
            vm.AddTransactionTypeCommand.Execute(null);
            var transactionType = vm.TransactionTypes.Last();

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

        protected Stock CreateUserStock(decimal balanceValue, int daysSinceLastStockEdit)
        {
            var app = _fixture.Container.Resolve<ApplicationViewModel>();
            app.SelectViewModelCommand.Execute(ViewModel.StockManager);
            var stockVm = (StocksViewModel)app.SelectedViewModel;
            stockVm.AddStockCommand.Execute(null);
            var userStock = stockVm.Stocks.Last();
            userStock.IsUserStock = true;
            userStock.Name = $"User stock no {stockVm.Stocks.Count + 1}";
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