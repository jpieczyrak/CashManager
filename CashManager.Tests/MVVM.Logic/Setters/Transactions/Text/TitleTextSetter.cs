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
    public class TitleTextSetter
    {
        private readonly Transaction[] _transactions =
        {
            new Transaction { Title = "Title 1" },
            new Transaction { Title = "Title 2" },
            new Transaction { Title = "Title 3" }
        };

        [Fact]
        public void TextSetter_DisabledSetter_NoChange()
        {
            //given
            var textSetter = new TextSetter(TextSetterType.Title);
            var command = TextSetterCommand.Create(textSetter);

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(_transactions.Select(x => x.Title), result.Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetter_Change()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "title";
            var textSetter = new TextSetter(TextSetterType.Title) { IsChecked = true, Value = targetText };
            var command = TextSetterCommand.Create(textSetter);
            var expected = Mapper.Map<Transaction[]>(Mapper.Map<CashManager.Data.DTO.Transaction[]>(_transactions));
            foreach (var transaction in expected) transaction.Title = targetText;

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected.Select(x => x.Title), result.Select(x => x.Title));
        }

        [Fact]
        public void TextSetter_EnabledSetterAppend_Appended()
        {
            //given
            MapperConfiguration.Configure();
            string targetText = "title";
            var textSetter = new TextSetter(TextSetterType.Title) { IsChecked = true, Value = targetText, AppendMode = true };
            var command = TextSetterCommand.Create(textSetter);
            var expected = Mapper.Map<Transaction[]>(Mapper.Map<CashManager.Data.DTO.Transaction[]>(_transactions));
            foreach (var transaction in expected) transaction.Title += targetText;

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected.Select(x => x.Title), result.Select(x => x.Title));
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
            var expected = new[] { "Ttitlele 1", "Ttitlele 2", "Ttitlele 3" };

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected, result.Select(x => x.Title));
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
            var expected = new[] { "Title X", "Title X", "Title X" };

            //when
            var result = command.Execute(_transactions);

            //then
            Assert.Equal(expected, result.Select(x => x.Title));
        }
    }
}