using System.Linq;

using CashManager.WPF.Model;
using CashManager.WPF.Model.Common;
using CashManager.WPF.Model.Selectors;
using CashManager.WPF.Model.Setters;

using Xunit;

namespace CashManager.Tests.MVVM.Logic.Setters.Positions.SinglePicker
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
            var command = CashManager.WPF.Logic.Commands.Setters.SingleSetterCommand.Create(selector);
            var positions = GetPositions();

            //when
            var result = command.Execute(positions);

            //then
            Assert.Equal(positions.Select(x => x.Parent.Type), result.Select(x => x.Parent.Type));
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
            var command = CashManager.WPF.Logic.Commands.Setters.SingleSetterCommand.Create(selector);
            var positions = GetPositions();

            //when
            var result = command.Execute(positions);

            //then
            Assert.All(result.Select(x => x.Parent.Type), type => Assert.Equal(type, _typeB));
        }

        private Position[] GetPositions()
        {
            return new[]
            {
                new Position { Parent = new Transaction { Type = _typeA } },
                new Position { Parent = new Transaction { Type = _typeA } },
                new Position { Parent = new Transaction { Type = _typeA } }
            };
        }

        private Selectable[] GetTransactionTypes() { return new[] { _typeA, _typeB }.Select(x => new Selectable(x)).ToArray(); }
    }
}