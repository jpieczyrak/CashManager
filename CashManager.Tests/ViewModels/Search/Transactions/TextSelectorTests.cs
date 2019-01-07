using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
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
        public void OnTitleFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            string searchString = vm.MatchingTransactions.First().Title;
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.Title.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.TitleFilter.Value = searchString;
            vm.State.TitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnNoteFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            string searchString = vm.MatchingTransactions.First().Note;
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.Note.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.NoteFilter.Value = searchString;
            vm.State.NoteFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnPositionTitleFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            var vm = _fixture.Container.Resolve<SearchViewModel>();
            vm.Update();
            string searchString = vm.MatchingTransactions.First().Positions.First().Title.ToUpper();
            vm.IsTransactionsSearch = true;
            var expected = vm.MatchingTransactions
                             .Where(x => x.Positions.Any(y => y.Title.ToLower().Contains(searchString.ToLower())))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.PositionTitleFilter.Value = searchString;
            vm.State.PositionTitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.MatchingTransactions);
            Assert.Equal(expected.Length, vm.MatchingTransactions.Count);
            Assert.Equal(expected, vm.MatchingTransactions.OrderBy(x => x.Id).ToArray());
        }
    }
}