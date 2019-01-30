using System.Linq;
using System.Text.RegularExpressions;

using CashManager.Logic.Commands.Filters;
using CashManager.Model;
using CashManager.Model.Selectors;

using Xunit;

namespace CashManager.Tests.MVVM.Logic.Filters.TextFilters
{
    public class PositionTextFilter
    {
        private readonly Position[] _positions =
        {
            new Position { Title = "1st Title" },
            new Position { Title = "2nd Title" },
            new Position { Title = "3rd Title" },
            new Position { Title = "4th Title" },
            new Position { Title = "5th title" },
        };

        [Fact]
        public void TextFilter_NotChecked_AllResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle);
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(_positions, results);
        }

        [Fact]
        public void TextFilter_Checked_AllResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(_positions, results);
        }

        [Fact]
        public void TextFilter_CheckedValueSet_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "Th" };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(_positions.Where(x => x.Title.Contains("th")), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetInverse_NotMatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "Th", DisplayOnlyNotMatching = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(_positions.Where(x => !x.Title.Contains("th")), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetCaseSensitive_NoResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "Th", IsCaseSensitive = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Empty(results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetCaseSensitive_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "Title", IsCaseSensitive = true };
            var filter = TextFilter.Create(selector);

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(_positions.Where(x => x.Title.Contains("Title")), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetRegex_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = ".*[2-4].*", IsRegex = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".*[2-4].*");

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(3, results.Count());
            Assert.Equal(_positions.Where(x => regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetRegexInverse_NotMatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = ".*[2-4].*", IsRegex = true, DisplayOnlyNotMatching = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".*[2-4].*");

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(2, results.Count());
            Assert.Equal(_positions.Where(x => !regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetWildcardStar_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "*th*", IsWildCard = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".*th.*");

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(2, results.Count());
            Assert.Equal(_positions.Where(x => regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetWildcardQuestion_MatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "?th Title", IsWildCard = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".{1}th Title");

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Single(results);
            Assert.Equal(_positions.Where(x => regex.IsMatch(x.Title)), results);
        }

        [Fact]
        public void TextFilter_CheckedValueSetWildcardQuestionInverse_NotMatchingResults()
        {
            //given
            var selector = new TextSelector(TextSelectorType.PositionTitle) { IsChecked = true, Value = "?th Title", IsWildCard = true, DisplayOnlyNotMatching = true };
            var filter = TextFilter.Create(selector);
            var regex = new Regex(".{1}th Title");

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(4, results.Count());
            Assert.Equal(_positions.Where(x => !regex.IsMatch(x.Title)), results);
        }
    }
}