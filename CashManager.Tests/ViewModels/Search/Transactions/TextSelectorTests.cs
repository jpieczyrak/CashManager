using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Search;

using Xunit;

namespace CashManager.Tests.ViewModels.Search.Transactions
{
    public class TextSelectorTests : ViewModelTests
    {
        [Fact]
        public void OnTitleFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            string searchString = vm.Transactions.First().Title;
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.Title.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.TitleFilter.Value = searchString;
            vm.State.TitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnNoteFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            string searchString = vm.Transactions.First().Note;
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.Note.ToLower().Contains(searchString.ToLower()))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.NoteFilter.Value = searchString;
            vm.State.NoteFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }

        [Fact]
        public void OnPositionTitleFilterChanged_SomeTransactions_MatchingTransactions()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<SearchViewModel>();
            string searchString = vm.Transactions.First().Positions.First().Title.ToUpper();
            vm.IsTransactionsSearch = true;
            var expected = vm.Transactions
                             .Where(x => x.Positions.Any(y => y.Title.ToLower().Contains(searchString.ToLower())))
                             .OrderBy(x => x.Id)
                             .ToArray();

            //when
            vm.State.PositionTitleFilter.Value = searchString;
            vm.State.PositionTitleFilter.IsChecked = true;

            //then
            Assert.NotEmpty(vm.Transactions);
            Assert.Equal(expected.Length, vm.Transactions.Length);
            Assert.Equal(expected, vm.Transactions.OrderBy(x => x.Id).ToArray());
        }
    }
}