using System.Linq;

using CashManager.WPF;
using CashManager.WPF.Model;
using CashManager.WPF.Model.Common;
using CashManager.WPF.Model.Selectors;
using CashManager.WPF.Model.Setters;

using Xunit;

namespace CashManager.Tests.MVVM.Logic.Setters.Transactions.MultiPicker
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
            var command = CashManager.WPF.Logic.Commands.Setters.MultiSetterCommand.Create(selector);
            var transactions = GetTransactions();

            //when
            var result = command.Execute(transactions);

            //then
            Assert.All(result.SelectMany(x => x.Positions.SelectMany(y => y.Tags)), tag => Assert.Equal(_tagA, tag));
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
            var command = CashManager.WPF.Logic.Commands.Setters.MultiSetterCommand.Create(selector);
            var transactions = GetTransactions();

            //when
            var result = command.Execute(transactions);

            //then
            var tags = result.SelectMany(x => x.Positions.SelectMany(y => y.Tags));
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
            var command = CashManager.WPF.Logic.Commands.Setters.MultiSetterCommand.Create(selector);
            var transactions = GetTransactions();

            //when
            var result = command.Execute(transactions);

            //then
            var tags = result.SelectMany(x => x.Positions.SelectMany(y => y.Tags));
            Assert.NotEmpty(tags);
            Assert.All(result.SelectMany(x => x.Positions), position =>
            {
                Assert.Contains(_tagA, position.Tags);
                Assert.Contains(_tagB, position.Tags);
            });
        }

        private Transaction[] GetTransactions()
        {
            return new[]
            {
                new Transaction
                {
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Tags = new[] { _tagA } }
                    }
                },
                new Transaction
                {
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Tags = new[] { _tagA } },
                        new Position { Tags = new[] { _tagA } }
                    }
                },
                new Transaction
                {
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Tags = new[] { _tagA } },
                        new Position { Tags = new[] { _tagA } },
                        new Position { Tags = new[] { _tagA } }
                    }
                }
            };
        }

        private Selectable[] GetTags() { return new[] { _tagA, _tagB, _tagC }.Select(x => new Selectable(x)).ToArray(); }
    }
}