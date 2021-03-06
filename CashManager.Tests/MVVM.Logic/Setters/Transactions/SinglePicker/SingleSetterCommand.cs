﻿using System.Linq;

using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Model.Selectors;
using CashManager.Model.Setters;

using Xunit;

namespace CashManager.Tests.MVVM.Logic.Setters.Transactions.SinglePicker
{
    public class SingleSetterCommand
    {
        private readonly TransactionType _typeA = new TransactionType { Name = "A" };
        private readonly TransactionType _typeB = new TransactionType { Name = "B" };

        [Fact]
        public void SinglePickerSetter_DisabledSetter_NoChange()
        {
            //given
            var selector = new SingleSetter(MultiPickerType.TransactionType, GetTransactionTypes());
            var command = CashManager.Logic.Commands.Setters.SingleSetterCommand.Create(selector);
            var transactions = GetTransactions();

            //when
            var result = command.Execute(transactions);

            //then
            Assert.Equal(transactions.Select(x => x.Type), result.Select(x => x.Type));
        }

        [Fact]
        public void SinglePickerSetter_EnabledSetter_Change()
        {
            //given
            var selector = new SingleSetter(MultiPickerType.TransactionType, GetTransactionTypes())
            {
                IsChecked = true,
                Selected = new Selectable(_typeB)
            };
            var command = CashManager.Logic.Commands.Setters.SingleSetterCommand.Create(selector);
            var transactions = GetTransactions();

            //when
            var result = command.Execute(transactions);

            //then
            Assert.All(result.Select(x => x.Type), type => Assert.Equal(type, _typeB));
        }

        private Transaction[] GetTransactions()
        {
            return new[]
            {
                new Transaction { Type = _typeA },
                new Transaction { Type = _typeA },
                new Transaction { Type = _typeA }
            };
        }

        private Selectable[] GetTransactionTypes() { return new[] { _typeA, _typeB }.Select(x => new Selectable(x)).ToArray(); }
    }
}