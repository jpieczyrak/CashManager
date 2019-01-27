using System;
using System.Linq;

using CashManager.Logic.Commands.Setters;
using CashManager.Model;
using CashManager.Model.Setters;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.MVVM.Logic.Setters.Positions.Date
{
    public class PositionDateSetter
    {
        private readonly Position[] _positions =
        {
            new Position
            {
                BookDate = DateTime.Today.AddDays(-5),
                Parent = new Transaction { BookDate = DateTime.Today.AddDays(-5) }
            },
            new Position
            {
                BookDate = DateTime.Today.AddDays(-4),
                Parent = new Transaction { BookDate = DateTime.Today.AddDays(-4) }
            },
            new Position
            {
                BookDate = DateTime.Today.AddDays(-3),
                Parent = new Transaction { BookDate = DateTime.Today.AddDays(-3) }
            }
        };

        [Fact]
        public void DateSetter_DisabledSetter_NoChange()
        {
            //given
            var dateSetter = new DateSetter(DateSetterType.BookDate);
            var command = DateSetterCommand.Create(dateSetter);

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(_positions, result);
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
            var result = command.Execute(_positions);

            //then
            Assert.All(result.Select(x => x.BookDate), time => Assert.Equal(targetDate, time));
            Assert.All(result.Select(x => x.Parent.BookDate), time => Assert.Equal(targetDate, time));
        }
    }
}