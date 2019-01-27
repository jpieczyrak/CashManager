using System.Linq;

using CashManager.WPF;
using CashManager.WPF.Logic.Commands.Filters;
using CashManager.WPF.Model.Common;
using CashManager.WPF.Model.Selectors;

using Xunit;

using MapperConfiguration = CashManager.WPF.Configuration.Mapping.MapperConfiguration;
using Position = CashManager.WPF.Model.Position;
using Tag = CashManager.WPF.Model.Tag;
using Transaction = CashManager.WPF.Model.Transaction;

namespace CashManager.Tests.MVVM.Logic.Filters.TextFilters
{
    public class TransactionMultiFilter
    {
        private static readonly Tag _tagA = new Tag { Name = "a" };
        private static readonly Tag _tagB = new Tag { Name = "b" };
        private static readonly Tag _tagC = new Tag { Name = "c" };

        private readonly Transaction[] _transactions =
        {
            new Transaction { Positions = new TrulyObservableCollection<Position>(new[] { new Position { Tags = new[] { _tagA } } }) },
            new Transaction { Positions = new TrulyObservableCollection<Position>(new[] { new Position { Tags = new[] { _tagB } } }) },
            new Transaction { Positions = new TrulyObservableCollection<Position>(new[] { new Position { Tags = new[] { _tagA, _tagC } } }) }
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
            var expected = new[] { _transactions[0], _transactions[2] };

            //when
            var results = filter.Execute(_transactions);

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
            var expected = new[] { _transactions[2] };

            //when
            var results = filter.Execute(_transactions);

            //then
            Assert.Equal(expected.Length, results.Count());
            Assert.Equal(expected, results);
        }
    }
}