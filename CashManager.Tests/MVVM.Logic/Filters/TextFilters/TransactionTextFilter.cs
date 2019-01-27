using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Logic.Commands.Filters;
using CashManager.Model;
using CashManager.Model.Selectors;

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
        public void TextFilter_CheckedValueSetInverse_NotMatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "Th", DisplayOnlyNotMatching = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(_transactions.Where(x => !x.Title.Contains("th")), results);
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
            Assert.Equal(3, results.Count());
            Assert.Equal(_transactions.Where(x => regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetRegexInverse_NotMatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = ".*[2-4].*", IsRegex = true, DisplayOnlyNotMatching = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".*[2-4].*");

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(2, results.Count());
            Assert.Equal(_transactions.Where(x => !regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetWildcardStar_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "*th*", IsWildCard = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".*th.*");

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(2, results.Count());
            Assert.Equal(_transactions.Where(x => regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetWildcardQuestion_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "?th Title", IsWildCard = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".{1}th Title");

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Single(results);
            Assert.Equal(_transactions.Where(x => regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetWildcardQuestionInverse_NotMatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.Title) { IsChecked = true, Value = "?th Title", IsWildCard = true, DisplayOnlyNotMatching = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".{1}th Title");

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(4, results.Count());
            Assert.Equal(_transactions.Where(x => !regex.IsMatch(x.Title)), results);
        }
    }
}