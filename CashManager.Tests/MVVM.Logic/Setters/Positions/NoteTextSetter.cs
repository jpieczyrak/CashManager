﻿using System.Linq;

using AutoMapper;

using CashManager_MVVM.Logic.Commands.Setters;
using CashManager_MVVM.Model.Selectors;
using CashManager_MVVM.Model.Setters;

using Xunit;

using MapperConfiguration = CashManager_MVVM.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.MVVM.Logic.Setters.Positions
{
    public class NoteTextSetter
    {
        private readonly CashManager_MVVM.Model.Transaction[] _transactions =
        {
            new CashManager_MVVM.Model.Transaction { Note = "Note 1" },
            new CashManager_MVVM.Model.Transaction { Note = "Note 2" },
            new CashManager_MVVM.Model.Transaction { Note = "Note 3" }
        };

        [Fact]
        public void TextSetter_DisabledSetter_NoChange()
        {
            //given
            var textSetter = new TextSetter(TextSetterType.Note);
            var command = TextSetterCommand.Create(textSetter);

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(_transactions.Select(x => x.Note), result.Select(x => x.Note));
        }

        [Fact]
        public void TextSetter_EnabledSetter_Change()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.Note) { IsChecked = true, Value = targetText };
            var command = TextSetterCommand.Create(textSetter);
            var expected = Mapper.Map<CashManager_MVVM.Model.Transaction[]>(Mapper.Map<CashManager.Data.DTO.Transaction[]>(_transactions));
            foreach (var transaction in expected) transaction.Note = targetText;

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected.Select(x => x.Note), result.Select(x => x.Note));
        }

        [Fact]
        public void TextSetter_EnabledSetterAppend_Appended()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "test";
            var textSetter = new TextSetter(TextSetterType.Note) { IsChecked = true, Value = targetText, AppendMode = true };
            var command = TextSetterCommand.Create(textSetter);
            var expected = Mapper.Map<CashManager_MVVM.Model.Transaction[]>(Mapper.Map<CashManager.Data.DTO.Transaction[]>(_transactions));
            foreach (var transaction in expected) transaction.Note += targetText;

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected.Select(x => x.Note), result.Select(x => x.Note));
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
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected, result.Select(x => x.Note));
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
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected, result.Select(x => x.Note));
        }
    }
}