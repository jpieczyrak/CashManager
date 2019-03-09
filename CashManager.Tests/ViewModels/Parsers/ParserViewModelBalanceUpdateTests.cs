using System;
using System.Linq;

using Autofac;

using CashManager.CommonData;
using CashManager.Features.Parsers;
using CashManager.Features.Parsers.Custom;
using CashManager.Features.TransactionTypes;
using CashManager.Infrastructure.DbConnection;
using CashManager.Logic.Parsers.Custom;
using CashManager.Model;
using CashManager.Properties;
using CashManager.Tests.ViewModels.Fixtures;

using LiteDB;

using Xunit;

namespace CashManager.Tests.ViewModels.Parsers
{
    [Collection("Cleanable database collection")]
    public class ParserViewModelBalanceUpdateTests
    {
        private readonly CleanableDatabaseFixture _fixture;

        public ParserViewModelBalanceUpdateTests(CleanableDatabaseFixture fixture)
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
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), "title A;20;20;05.10.2010");

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(20, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Equal(20, stock.Balance.Value);
            Assert.Equal(new DateTime(2010, 10, 5), vm.Parser.Balances.First().Value.Max(x => x.Key));
        }

        [Fact]
        public void Save_ValueMatchingBalance_NoCorrection()
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), "title A;20;20;05.10.2010");

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Single(vm.TransactionsProvider.AllTransactions);
            Assert.Equal(20, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Equal(20, stock.Balance.Value);
            Assert.Equal(new DateTime(2010, 10, 5), vm.Parser.Balances.First().Value.Max(x => x.Key));
        }

        [Theory]
        [InlineData(20, 100)]
        [InlineData(20, 32)]
        [InlineData(-20, 60)]
        [InlineData(-20, -30)]
        [InlineData(20, -20)]
        [InlineData(500, 700)]
        public void Save_ValueNotMatchingBalance_CorrectionAdded(decimal transactionValue, decimal balance)
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), $"title A;{transactionValue};{balance};05.10.2010");

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Equal(2, vm.TransactionsProvider.AllTransactions.Count);
            Assert.Equal(balance, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Equal(balance, stock.Balance.Value);
            Assert.Equal(new DateTime(2010, 10, 5), vm.Parser.Balances.First().Value.Max(x => x.Key));
            Assert.Equal(balance - transactionValue, vm.TransactionsProvider.AllTransactions.FirstOrDefault(x => x.Title == Strings.Correction).ValueAsProfit);
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
            Assert.Equal(0, vm.SelectedUserStock.Balance.Value);
            Assert.Equal(new DateTime(2015, 5, 5), vm.SelectedUserStock.Balance.BookDate);

            //fakly created stock should not exists in db when it was NOT updated
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Null(stock);
        }

        [Fact]
        public void DoubleSave_ImportingAlreadyImportedDataWithMatchingBalance_SingleBalanceUpdateNoCorrection()
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), "title A;100;100;05.10.2010");

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

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
        public void DoubleSave_ImportingExtendedAlreadyImportedDataWithMatchingBalance_SingleBalanceUpdateNoCorrection()
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), "title A;100;100;05.10.2010");

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            vm.InputText += "\r\ntitle B;100;200;06.10.2010";

            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Equal(2, vm.TransactionsProvider.AllTransactions.Count);
            Assert.Equal(200, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Equal(200, stock.Balance.Value);
            Assert.Equal(new DateTime(2010, 10, 6), vm.Parser.Balances.First().Value.Max(x => x.Key));
        }

        [Fact]
        public void DoubleSave_ImportingExtendedAlreadyImportedDataWithNotMatchingBalance_SingleBalanceUpdateAndCorrection()
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), "title A;20;100;05.10.2010"); //-> +80 correction

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            vm.InputText += "\r\ntitle B;100;200;06.10.2010";

            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Equal(3, vm.TransactionsProvider.AllTransactions.Count);
            Assert.Equal(200, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Equal(200, stock.Balance.Value);
            Assert.Equal(new DateTime(2010, 10, 6), vm.Parser.Balances.First().Value.Max(x => x.Key));

            Assert.Equal(80m, vm.TransactionsProvider.AllTransactions.FirstOrDefault(x => x.Title == Strings.Correction).ValueAsProfit);
        }

        [Fact]
        public void DoubleSave_ImportingExtendedAlreadyImportedDataWithNotMatchingBalance2_SingleBalanceUpdateAndCorrection()
        {
            //given
            var db = _fixture.Container.Resolve<LiteRepository>().Database;
            var vm = PrepareParserViewModel(new DateTime(2000, 5, 5), "title A;20;100;05.10.2010"); //-> +80 correction

            //when
            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            vm.InputText += "\r\ntitle B;100;-100;06.10.2010"; // -> - 300 correction

            Assert.True(vm.ParseCommand.CanExecute(null));
            vm.ParseCommand.Execute(null);
            Assert.True(vm.SaveCommand.CanExecute(null));
            vm.SaveCommand.Execute(null);

            //then
            Assert.Equal(4, vm.TransactionsProvider.AllTransactions.Count);
            Assert.Equal(-100, vm.Parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
            var stock = db.Query<CashManager.Data.DTO.Stock>(x => x.Id == vm.SelectedUserStock.Id).FirstOrDefault();
            Assert.Equal(-100, stock.Balance.Value);
            Assert.Equal(new DateTime(2010, 10, 6), vm.Parser.Balances.First().Value.Max(x => x.Key));


            var corrections = vm.TransactionsProvider.AllTransactions.Where(x => x.Title == Strings.Correction).OrderBy(x => x.InstanceCreationDate).ToArray();
            Assert.Equal(80m, corrections[0].ValueAsProfit);
            Assert.Equal(-300m, corrections[1].ValueAsProfit);
        }

        private CsvParserViewModel PrepareParserViewModel(DateTime bookDate, string inputText)
        {
            var typeVm = _fixture.Container.Resolve<TransactionTypesViewModel>();
            typeVm.AddTransactionTypeCommand.Execute(null);
            typeVm.AddTransactionTypeCommand.Execute(null);
            typeVm.TransactionTypes[1].Income= true;
            typeVm.TransactionTypes[1].Outcome = false;

            var userStock = new Stock
            {
                Name = "1",
                IsUserStock = true,
                Balance = new Balance
                {
                    Value = 0m
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