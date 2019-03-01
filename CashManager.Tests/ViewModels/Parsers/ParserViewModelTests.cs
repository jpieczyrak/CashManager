using System;
using System.Linq;

using Autofac;

using CashManager.CommonData;
using CashManager.Data.ViewModelState.Parsers;
using CashManager.Features.Parsers;
using CashManager.Features.Parsers.Custom;
using CashManager.Infrastructure.DbConnection;
using CashManager.Logic.Parsers.Custom;
using CashManager.Logic.Parsers.Custom.Predefined;
using CashManager.Model;
using CashManager.Tests.ViewModels.Fixtures;

using LiteDB;

using Xunit;

namespace CashManager.Tests.ViewModels.Parsers
{
    [Collection("Cleanable database collection")]
    public class ParserViewModelTests
    {
        private readonly CleanableDatabaseFixture _fixture;

        public ParserViewModelTests(CleanableDatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.Container.Resolve<TransactionsProvider>().AllTransactions.Clear();
            _fixture.Reset();
        }

        [Fact]
        public void Save_StockBalanceDateBeforeImportedDate_BalanceUpdated()
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), "title A;20;100;05.10.2010");

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(100, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Equal(100, stock.Balance.Value);
            Assert.Equal(new DateTime(2010, 10, 5), vm.Parser.Balances.First().Value.Max(x => x.Key));
        }

        [Fact]
        public void Save_StockBalanceDateAfterImportedDate_BalanceNoUpdated()
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2015, 5, 5), "title A;20;100;05.10.2010");

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(100, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            Assert.Equal(10, vm.SelectedUserStock.Balance.Value);
            Assert.Equal(new DateTime(2015, 5, 5), vm.SelectedUserStock.Balance.BookDate);

            //fakly created stock should not exists in db when it was NOT updated
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Null(stock);
        }

        private CsvParserViewModel PrepareParserViewModel(DateTime bookDate, string inputText)
        {
            var userStock = new Stock
            {
                Name = "1",
                IsUserStock = true,
                Balance = new Balance
                {
                    Value = 10m
                }
            };
            userStock.Balance.BookDate = bookDate;
            var vm = _fixture.Container.Resolve<CsvParserViewModel>();
            vm.UserStocks = new[] { userStock };
            vm.AddRuleCommand.Execute(null);
            vm.AddRuleCommand.Execute(null);
            vm.AddRuleCommand.Execute(null);
            vm.Rules[0].Property = TransactionField.Title;
            vm.Rules[1].Property = TransactionField.Value;
            vm.Rules[2].Property = TransactionField.Balance;
            vm.Rules[3].Property = TransactionField.BookDate;
            vm.SelectedUpdateBalanceMode = ParserUpdateBalanceMode.IfNewer;
            vm.InputText = inputText;
            vm.DefaultIncomeTransactionType = new TransactionType { Income = true, Outcome = false, Name = "Income" };
            vm.DefaultOutcomeTransactionType = new TransactionType { Income = false, Outcome = true, Name = "Outcome" };
            vm.SelectedUserStock = userStock;
            return vm;
        }
    }
}