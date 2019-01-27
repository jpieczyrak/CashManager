using System.Linq;

using CashManager.Logic.Commands.Setters;
using CashManager.Model;
using CashManager.Model.Selectors;
using CashManager.Model.Setters;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.MVVM.Logic.Setters.Positions.Text
{
    public class NoteTextSetter
    {
        private readonly Position[] _positions =
        {
            new Position { Parent = new Transaction { Note = "Note 1" } },
            new Position { Parent = new Transaction { Note = "Note 2" } },
            new Position { Parent = new Transaction { Note = "Note 3" } }
        };

        public NoteTextSetter()
        {
            foreach (var position in _positions)
                position.Parent.Positions = new TrulyObservableCollection<Position> { position };
        }

        [Fact]
        public void TextSetter_DisabledSetter_NoChange()
        {
            //given
            var textSetter = new TextSetter(TextSetterType.Note);
            var command = TextSetterCommand.Create(textSetter);

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(_positions.Select(x => x.Parent.Note), result.Select(x => x.Parent.Note));
        }

        [Fact]
        public void TextSetter_EnabledSetter_Change()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.Note) { IsChecked = true, Value = targetText };
            var command = TextSetterCommand.Create(textSetter);
            var expected = new [] { "test", "test", "test" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Note));
        }

        [Fact]
        public void TextSetter_EnabledSetterAppend_Appended()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.Note) { IsChecked = true, Value = targetText, AppendMode = true };
            var command = TextSetterCommand.Create(textSetter);
            var expected = new[] { "Note 1test", "Note 2test", "Note 3test" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Note));
        }

        [Fact]
        public void TextSetter_EnabledSetterReplaceMatch_OnlyMatchReplaced()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "x";
            var textSelector = new TextSelector(TextSelectorType.Note) { IsChecked = true, Value = "ot" };
            var textSetter = new TextSetter(TextSetterType.Note) { IsChecked = true, Value = targetText, ReplaceMatch = true };
            var command = TextSetterCommand.Create(textSetter, textSelector);
            var expected = new[] { "Nxe 1", "Nxe 2", "Nxe 3" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Note));
        }

        [Fact]
        public void TextSetter_EnabledSetterReplaceRegexMatch_OnlyMatchReplaced()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "X";
            var textSelector = new TextSelector(TextSelectorType.Note) { IsChecked = true, Value = @"\d", IsRegex = true };
            var textSetter = new TextSetter(TextSetterType.Note) { IsChecked = true, Value = targetText, ReplaceMatch = true };
            var command = TextSetterCommand.Create(textSetter, textSelector);
            var expected = new[] { "Note X", "Note X", "Note X" };

            //when
            var result = command.Execute(_positions);

            //then
            Assert.Equal(expected, result.Select(x => x.Parent.Note));
        }
    }
}