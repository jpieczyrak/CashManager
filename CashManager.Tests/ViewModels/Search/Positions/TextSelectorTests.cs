using System.Linq;

using Autofac;

using CashManager.Tests.ViewModels.Fixtures;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
{
    [Collection("Database collection")]
    public class TextSelectorTests
    {
        private readonly DatabaseFixture _fixture;

        public TextSelectorTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnTitleFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            string searchString = vm.MatchingTransactions.First().Title;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelContext.Positions.Value
                             .Where(x => x.Parent.Title.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.TitleFilter.Value = searchString;
            vm.State.TitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnNoteFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            string searchString = vm.MatchingTransactions.First().Note;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelContext.Positions.Value
                             .Where(x => x.Parent.Note.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.NoteFilter.Value = searchString;
            vm.State.NoteFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnPositionTitleFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            string searchString = vm.MatchingTransactions.First().Positions.First().Title.ToUpper();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = _fixture.ViewModelContext.Positions.Value
                             .Where(x => x.Title.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.PositionTitleFilter.Value = searchString;
            vm.State.PositionTitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingPositions);
            Assert.Equal(expected.Length, vm.MatchingPositions.Count);
            Assert.Equal(expected, vm.MatchingPositions.OrderBy(x => x.Id).ToArray());
        }
    }
}