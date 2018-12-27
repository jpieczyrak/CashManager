using System.Collections.Generic;

using Autofac;

using CashManager_MVVM;
using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Features.Parsers;
using CashManager_MVVM.Model;

using Xunit;

namespace CashManager.Tests.ViewModels
{
    public class ParserViewModelTests : ViewModelTests
    {
        [Fact]
        public void SaveCommandExecute_ValidTransactions_TransactionsAreBeingAddedToCommonState()
        {
            //given
            var vm = _container.Resolve<ParseViewModel>();
            var transaction = new Transaction
            {
                Title = "title 1",
                Positions = new TrulyObservableCollection<Position>(new[] { Positions[0] }),
                Type = Types[0]
            };
            transaction.Positions[0].TagViewModel = _container.Resolve<MultiComboBoxViewModel>();
            transaction.Positions[0].TagViewModel.SetInput(Tags);
            var transactions = new List<Transaction> { transaction, transaction };
            vm.InputText = "sth";
            vm.ParseCommand.Execute(null);
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
            var vm = _container.Resolve<ParseViewModel>();
            string title = "title 1";
            var transaction = new Transaction
            {
                Title = title,
                Positions = new TrulyObservableCollection<Position>(new[] { Positions[0] }),
                Type = Types[0]
            };
            transaction.Positions[0].TagViewModel = _container.Resolve<MultiComboBoxViewModel>();
            transaction.Positions[0].TagViewModel.SetInput(Tags);
            var transactions = new List<Transaction> { transaction, transaction };
            vm.InputText = "sth";
            vm.ParseCommand.Execute(null);
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