using System.Linq;

using CashManager.Logic.Commands.Setters;
using CashManager.Model;
using CashManager.Model.Selectors;
using CashManager.Model.Setters;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.MVVM.Logic.Setters.Positions.Text
{
    public class PositionTitleTextSetter
    {
        private readonly Position[] _positions =
        {
            new Position { Title = "Title 1" },
            new Position { Title = "Title 2" },
            new Position { Title = "Title 3" }
        };

        [Fact]
        public void TextSetter_DisabledSetter_NoChange()
        {
            //given
            var textSetter = new TextSetter(TextSetterType.PositionTitle);
            var command = TextSetterCommand.Create(textSetter);

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(_positions.Select(x => x.Title), result.Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetter_Change()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.PositionTitle) { IsChecked = true, Value = targetText };
            var command = TextSetterCommand.Create(textSetter);
            var expected = new[] { targetText, targetText, targetText };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterAppend_Appended()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.PositionTitle) { IsChecked = true, Value = targetText, AppendMode = true };
            var command = TextSetterCommand.Create(textSetter);
            var expected = new[] { _positions[0].Title + targetText, _positions[1].Title + targetText, _positions[2].Title + targetText };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterReplaceMatch_OnlyMatchReplaced()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "x";
            var textSelector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "it" };
            var textSetter = new TextSetter(TextSetterType.PositionTitle) { IsChecked = true, Value = targetText, ReplaceMatch = true };
            var command = TextSetterCommand.Create(textSetter, textSelector);
            var expected = new[] { "Txle 1", "Txle 2", "Txle 3" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterReplaceRegexMatch_OnlyMatchReplaced()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "X";
            var textSelector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = @"\d", IsRegex = true };
            var textSetter = new TextSetter(TextSetterType.PositionTitle) { IsChecked = true, Value = targetText, ReplaceMatch = true };
            var command = TextSetterCommand.Create(textSetter, textSelector);
            var expected = new[] { "Title X", "Title X", "Title X" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Title));
        }
    }
}