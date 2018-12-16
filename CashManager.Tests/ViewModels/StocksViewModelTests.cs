using System.Linq;

using Autofac;

using CashManager_MVVM.Features.Stocks;

using Xunit;

namespace CashManager.Tests.ViewModels
{
    public class StocksViewModelTests : ViewModelTests
    {
        [Fact]
        public void RemoveCommandExecute_FirstStock_FirstStockIsRemoved()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<StocksViewModel>();
            var expectedStocks = vm.Stocks.Skip(1).OrderBy(x => x.Name).ToArray();

            //when
            vm.RemoveCommand.Execute(vm.Stocks[0]);

            //then
            Assert.Equal(expectedStocks, vm.Stocks.OrderBy(x => x.Name).ToArray());
        }
    }
}