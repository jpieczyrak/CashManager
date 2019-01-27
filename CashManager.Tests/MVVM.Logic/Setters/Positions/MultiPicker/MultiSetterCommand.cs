using System.Linq;

using CashManager.WPF.Model;
using CashManager.WPF.Model.Common;
using CashManager.WPF.Model.Selectors;
using CashManager.WPF.Model.Setters;

using Xunit;

namespace CashManager.Tests.MVVM.Logic.Setters.Positions.MultiPicker
{
    public class MultiSetterCommand
    {
        private readonly Tag _tagA = new Tag { Name = "A" };
        private readonly Tag _tagB = new Tag { Name = "B" };
        private readonly Tag _tagC = new Tag { Name = "C" };

        [Fact]
        public void MultiSetter_DisabledSetter_NoChange()
        {
            //given
            var selector = new MultiSetter(MultiPickerType.Tag, GetTags());
            var command = WPF.Logic.Commands.Setters.MultiSetterCommand.Create(selector);
            var positions = GetPositions();

            //when
            var result = command.Execute(positions);

            //then
            Assert.All(result.SelectMany(x => x.Tags), tag => Assert.Equal(_tagA, tag));
        }

        [Fact]
        public void MultiSetter_EnabledSetter_Change()
        {
            //given
            var selector = new MultiSetter(MultiPickerType.Tag, GetTags(),
                new[] { new Selectable(_tagB) })
            {
                IsChecked = true
            };
            var command = WPF.Logic.Commands.Setters.MultiSetterCommand.Create(selector);
            var positions = GetPositions();

            //when
            var result = command.Execute(positions);

            //then
            var tags = result.SelectMany(x => x.Tags);
            Assert.NotEmpty(tags);
            Assert.All(tags, tag => Assert.Equal(_tagB, tag));
        }

        [Fact]
        public void MultiSetter_EnabledSetterAppend_Change()
        {
            //given
            var selector = new MultiSetter(MultiPickerType.Tag, GetTags(),
                new[] { new Selectable(_tagB) })
            {
                IsChecked = true,
                Append = true
            };
            var command = WPF.Logic.Commands.Setters.MultiSetterCommand.Create(selector);
            var positions = GetPositions();

            //when
            var result = command.Execute(positions);

            //then
            var tags = result.SelectMany(x => x.Tags);
            Assert.NotEmpty(tags);
            Assert.All(result, position =>
            {
                Assert.Contains(_tagA, position.Tags);
                Assert.Contains(_tagB, position.Tags);
            });
        }

        private Position[] GetPositions()
        {
            var parentA = new Transaction();
            var parentB = new Transaction();
            return new[]
            {
                new Position { Tags = new[] { _tagA }, Parent = parentA },
                new Position { Tags = new[] { _tagA }, Parent = parentA },
                new Position { Tags = new[] { _tagA }, Parent = parentA },
                new Position { Tags = new[] { _tagA }, Parent = parentB },
                new Position { Tags = new[] { _tagA }, Parent = parentB }
            };
        }

        private Selectable[] GetTags() { return new[] { _tagA, _tagB, _tagC }.Select(x => new Selectable(x)).ToArray(); }
    }
}