using System.Collections.Generic;
using System.Linq;

using Autofac;

using CashManager.Features.Common;
using CashManager.Features.Parsers;
using CashManager.Logic.Parsers;
using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Tests.ViewModels.Fixtures;

using Xunit;

namespace CashManager.Tests.ViewModels
{
    [Collection("Cleanable database collection")]
    public class ParserViewModelTests
    {
        private readonly Tag[] _tags = { new Tag(), new Tag() };
        private readonly CleanableDatabaseFixture _fixture;

        public ParserViewModelTests(CleanableDatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.Reset();
        }

        [Fact]
        public void SaveCommandExecute_ValidTransactions_TransactionsAreBeingAddedToCommonState()
        {
            //given
            var vm = _fixture.Container.Resolve<ParserViewModelBase>();
            vm.Parser = new GetinBankParser();
            vm.SelectedUserStock = vm.SelectedExternalStock = new Stock();
            var transaction = new Transaction
            {
                Title = "title 1",
                Positions = new TrulyObservableCollection<Position>(new[] { new Position() }),
                Type = new TransactionType()
            };
            transaction.Positions[0].TagViewModel = _fixture.Container.Resolve<MultiComboBoxViewModel>();
            transaction.Positions[0].TagViewModel.SetInput(_tags.Select(x => new Selectable(x)));
            var transactions = new List<Transaction> { transaction, Transaction.Copy(transaction) };
            vm.InputText = "06.09.2016 – PRZELEW WYCHODZĄCY\r\nJĘDRZEJ PIECZYRAK – [Sierpień] Czynsz + media\r\n\r\n-684,62 PLN";
            vm.ResultsListViewModel.Transactions.AddRange(transactions);
            var command = vm.SaveCommand;

            //when
            bool canExecute = command.CanExecute(null);
            command.Execute(null);

            //then
            Assert.True(canExecute);
            Assert.Equal(transactions.Count, vm.TransactionsProvider.AllTransactions.Count);
        }

        [Fact]
        public void SaveCommandExecute_ValidTransactions_TransactionsAreBeingUpdatedToCommonState()
        {
            //given
            var vm = _fixture.Container.Resolve<ParserViewModelBase>();
            vm.Parser = new GetinBankParser();
            string title = "title 1";
            var transaction = new Transaction
            {
                Title = title,
                Positions = new TrulyObservableCollection<Position>(new[] { new Position() }),
                Type = new TransactionType()
            };
            transaction.Positions[0].TagViewModel = _fixture.Container.Resolve<MultiComboBoxViewModel>();
            transaction.Positions[0].TagViewModel.SetInput(_tags.Select(x => new Selectable(x)));
            var transactions = new List<Transaction> { transaction, Transaction.Copy(transaction) };
            vm.InputText = "     06.09.2016 – PRZELEW WYCHODZĄCY\r\nJĘDRZEJ PIECZYRAK – [Sierpień] Czynsz + media\r\n\r\n-684,62 PLN";
            vm.ResultsListViewModel.Transactions.AddRange(transactions);
            var command = vm.SaveCommand;
            command.Execute(null);

            transactions[0].Title += 1;
            vm.ResultsListViewModel.Transactions.Clear();
            vm.ResultsListViewModel.Transactions.AddRange(transactions);

            //when
            bool canExecute = command.CanExecute(null);
            command.Execute(null);

            //then
            Assert.True(canExecute);
            Assert.Equal(transactions.Count, vm.TransactionsProvider.AllTransactions.Count);
            Assert.Equal(transactions[0].Title, vm.TransactionsProvider.AllTransactions[0].Title);
        }
    }
}