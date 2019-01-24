﻿using System.Linq;
using System.Text.RegularExpressions;

using CashManager_MVVM.Logic.Commands;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using Xunit;

namespace CashManager.Tests.MVVM.Logic.Filters.TextFilters
{
    public class TransactionTextFilter
    {
        private readonly Transaction[] _transactions =
        {
            new Transaction { Title = "1st Title" },
            new Transaction { Title = "2nd Title" },
            new Transaction { Title = "3rd Title" },
            new Transaction { Title = "4th Title" },
            new Transaction { Title = "5th title" },
        };

        [Fact]
        public void TextFilter_NotChecked_AllResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title);
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(_transactions, results);
        }

        [Fact]
        public void TextFilter_Checked_AllResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(_transactions, results);
        }

        [Fact]
        public void TextFilter_CheckedValueSet_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "Th" };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(_transactions.Where(x => x.Title.Contains("th")), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetCaseSensitive_NoResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "Th", IsCaseSensitive = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Empty(results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetCaseSensitive_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "Title", IsCaseSensitive = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(_transactions.Where(x => x.Title.Contains("Title")), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetRegex_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = ".*[2-4].*", IsRegex = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".*[2-4].*");

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(_transactions.Where(x => regex.IsMatch(x.Title.ToLower())), results);
            Assert.Equal(3, results.Count());
        }
    }
}