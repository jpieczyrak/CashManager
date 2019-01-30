using System.Linq;

using CashManager.Logic.Commands.Filters;
using CashManager.Model.Common;
using CashManager.Model.Selectors;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;
using Position = CashManager.Model.Position;
using Tag = CashManager.Model.Tag;

namespace CashManager.Tests.MVVM.Logic.Filters.TextFilters
{
    public class PositionMultiFilter
    {
        private static readonly Tag _tagA = new Tag { Name = "a" };
        private static readonly Tag _tagB = new Tag { Name = "b" };
        private static readonly Tag _tagC = new Tag { Name = "c" };

        private readonly Position[] _positions =
        {
            new Position { Tags = new[] { _tagA } },
            new Position { Tags = new[] { _tagB } },
            new Position { Tags = new[] { _tagA, _tagC } }
        };

        private readonly Tag[] _tags =
        {
            _tagA,
            _tagB,
            _tagC
        };

        [Fact]
        public void MultiPickerFilter_CheckedValueSetMatchOneOf_MatchingResults()
        {
            //given
            var selectedValues = new[] { _tagA, _tagC };
            MapperConfiguration.Configure();
            var source = _tags.Select(x => new Selectable(x)).ToArray();
            var selector = new MultiPicker(MultiPickerType.Tag, source) { IsChecked = true };

            foreach (var result in selector.ComboBox.InternalDisplayableSearchResults)
                if (selectedValues.Any(x => x.Id == result.Id))
                    result.IsSelected = true;

            var filter = MultiPickerFilter.Create(selector);
            var expected = new[] { _positions[0], _positions[2] };

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(expected.Length, results.Count());
            Assert.Equal(expected, results);
        }

        [Fact]
        public void MultiPickerFilter_CheckedValueSetMatchAllOf_MatchingResults()
        {
            //given
            var selectedValues = new[] { _tagA, _tagC };
            MapperConfiguration.Configure();
            var source = _tags.Select(x => new Selectable(x)).ToArray();
            var selector = new MultiPicker(MultiPickerType.Tag, source) { IsChecked = true, CanMatchMultipleElements = true, ShouldMatchAllOfTheElements = true };

            foreach (var result in selector.ComboBox.InternalDisplayableSearchResults)
                if (selectedValues.Any(x => x.Id == result.Id))
                    result.IsSelected = true;

            var filter = MultiPickerFilter.Create(selector);
            var expected = new[] { _positions[2] };

            //when
            var results = filter.Execute(_positions);

            //then
            Assert.Equal(expected.Length, results.Count());
            Assert.Equal(expected, results);
        }
    }
}