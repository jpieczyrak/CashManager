﻿using System.Linq;

using AutoMapper;

using CashManager.Logic.Commands.Setters;
using CashManager.Model;
using CashManager.Model.Selectors;
using CashManager.Model.Setters;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.MVVM.Logic.Setters.Transactions.Text
{
    public class PositionTitleTextSetter
    {
        private readonly Transaction[] _transactions =
        {
            new Transaction { Positions = new TrulyObservableCollection<Position> { new Position { Title = "Title 1" } } },

            new Transaction { Positions = new TrulyObservableCollection<Position> { new Position { Title = "Title 2" } } },
            new Transaction
            {
                Positions = new TrulyObservableCollection<Position>
                {
                    new Position { Title = "Title 3" },
                    new Position { Title = "Title 4" }
                }
            }
        };

        [Fact]
        public void TextSetter_DisabledSetter_NoChange()
        {
            //given
            var textSetter = new TextSetter(TextSetterType.PositionTitle);
            var command = TextSetterCommand.Create(textSetter);

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(_transactions.SelectMany(x => x.Positions).Select(x => x.Title), result.SelectMany(x => x.Positions).Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetter_Change()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.PositionTitle) { IsChecked = true, Value = targetText };
            var command = TextSetterCommand.Create(textSetter);
            var expected = Mapper.Map<Transaction[]>(Mapper.Map<CashManager.Data.DTO.Transaction[]>(_transactions));
            foreach (var position in expected.SelectMany(x => x.Positions)) position.Title = targetText;

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected.SelectMany(x => x.Positions).Select(x => x.Title), result.SelectMany(x => x.Positions).Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterAppend_Appended()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.PositionTitle) { IsChecked = true, Value = targetText, AppendMode = true };
            var command = TextSetterCommand.Create(textSetter);
            var expected = Mapper.Map<Transaction[]>(Mapper.Map<CashManager.Data.DTO.Transaction[]>(_transactions));
            foreach (var position in expected.SelectMany(x => x.Positions)) position.Title += targetText;

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected.SelectMany(x => x.Positions).Select(x => x.Title), result.SelectMany(x => x.Positions).Select(x => x.Title));
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
            var expected = new[] { "Txle 1", "Txle 2", "Txle 3", "Txle 4" };

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected, result.SelectMany(x => x.Positions).Select(x => x.Title));
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
            var expected = new[] { "Title X", "Title X", "Title X", "Title X" };

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected, result.SelectMany(x => x.Positions).Select(x => x.Title));
        }
    }
}