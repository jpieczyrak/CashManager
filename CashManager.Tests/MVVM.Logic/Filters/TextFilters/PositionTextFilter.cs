using System.Linq;

using CashManager_MVVM.Logic.Commands;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

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
    }
}