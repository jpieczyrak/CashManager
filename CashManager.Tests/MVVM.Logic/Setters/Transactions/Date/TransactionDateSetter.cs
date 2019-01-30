using System;
using System.Linq;

using AutoMapper;

using CashManager.Logic.Commands.Setters;
using CashManager.Model;
using CashManager.Model.Setters;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.MVVM.Logic.Setters.Transactions.Date
{
    public class TransactionDateSetter
    {
        private readonly Transaction[] _transactions =
        {
            new Transaction { BookDate = DateTime.Today.AddDays(-5) },
            new Transaction { BookDate = DateTime.Today.AddDays(-4) },
            new Transaction { BookDate = DateTime.Today.AddDays(-3) }
        };

        [Fact]
        public void DateSetter_DisabledSetter_NoChange()
        {
            //given
            var dateSetter = new DateSetter(DateSetterType.BookDate);
            var command = DateSetterCommand.Create(dateSetter);

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(_transactions, result);
        }

        [Fact]
        public void DateSetter_EnabledSetter_Change()
        {
            //given
            MapperConfiguration.Configure();
            var targetDate = DateTime.Today.AddDays(-1);
            var dateSetter = new DateSetter(DateSetterType.BookDate) { IsChecked = true, Value = targetDate};
            var command = DateSetterCommand.Create(dateSetter);

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.All(result.Select(x => x.BookDate), time => Assert.Equal(targetDate, time));
            var expected = Mapper.Map<Transaction[]>(Mapper.Map<CashManager.Data.DTO.Transaction[]>(_transactions));
            foreach (var transaction in expected) transaction.BookDate = targetDate;
            Assert.Equal(expected, result);
        }
    }
}