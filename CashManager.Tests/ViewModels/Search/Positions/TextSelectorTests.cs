using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Positions
{
    public class TextSelectorTests : ViewModelTests
    {
        [Fact]
        public void OnTitleFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            string searchString = vm.Transactions.First().Title;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => x.Parent.Title.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.TitleFilter.Value = searchString;
            vm.State.TitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnNoteFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            string searchString = vm.Transactions.First().Note;
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => x.Parent.Note.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.NoteFilter.Value = searchString;
            vm.State.NoteFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnPositionTitleFilterChanged_SomePositions_MatchingPositions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            string searchString = vm.Transactions.First().Positions.First().Title.ToUpper();
            vm.IsPositionsSearch = true;
            vm.IsTransactionsSearch = false;
            var expected = Positions
                             .Where(x => x.Title.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.PositionTitleFilter.Value = searchString;
            vm.State.PositionTitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Positions);
            Assert.Equal(expected.Length, vm.Positions.Count);
            Assert.Equal(expected, vm.Positions.OrderBy(x => x.Id).ToArray());
        }
    }
}