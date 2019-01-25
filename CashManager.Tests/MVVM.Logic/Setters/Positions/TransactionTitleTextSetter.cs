using System.Linq;

using CashManager_MVVM.Logic.Commands.Setters;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;
using CashManager_MVVM.Model.Setters;

using Xunit;

using MapperConfiguration = CashManager_MVVM.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.MVVM.Logic.Setters.Positions
{
    public class TransactionTitleTextSetter
    {
        private static readonly Transaction _commonParent = new Transaction { Title = "Title 3" };
        private readonly Position[] _positions =
        {
            new Position { Parent = new Transaction { Title = "Title 1" } },

            new Position { Parent = new Transaction { Title = "Title 2" } },
            new Position { Parent = _commonParent },
            new Position { Parent = _commonParent },
        };


        [Fact]
        public void TextSetter_DisabledSetter_NoChange()
        {
            //given
            var textSetter = new TextSetter(TextSetterType.Title);
            var command = TextSetterCommand.Create(textSetter);

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(_positions.Select(x => x.Parent.Title), result.Select(x => x.Parent.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetter_Change()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "title";
            var textSetter = new TextSetter(TextSetterType.Title) { IsChecked = true, Value = targetText };
            var command = TextSetterCommand.Create(textSetter);
            var expected = new[] { targetText, targetText, targetText, targetText };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterAppend_Appended()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "title";
            var textSetter = new TextSetter(TextSetterType.Title) { IsChecked = true, Value = targetText, AppendMode = true };
            var command = TextSetterCommand.Create(textSetter);
            var expected = _positions.Select(x => $"{x.Parent.Title}{targetText}").ToArray();

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterReplaceMatch_OnlyMatchReplaced()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "title";
            var textSelector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "it" };
            var textSetter = new TextSetter(TextSetterType.Title) { IsChecked = true, Value = targetText, ReplaceMatch = true };
            var command = TextSetterCommand.Create(textSetter, textSelector);
            var expected = new[] { "Ttitlele 1", "Ttitlele 2", "Ttitlele 3", "Ttitlele 3" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterReplaceRegexMatch_OnlyMatchReplaced()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "X";
            var textSelector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = @"\d", IsRegex = true };
            var textSetter = new TextSetter(TextSetterType.Title) { IsChecked = true, Value = targetText, ReplaceMatch = true };
            var command = TextSetterCommand.Create(textSetter, textSelector);
            var expected = new[] { "Title X", "Title X", "Title X", "Title X" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Title));
        }
    }
}