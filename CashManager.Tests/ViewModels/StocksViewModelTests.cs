using System.Linq;

using Autofac;

using CashManager.Features.Stocks;
using CashManager.Model;
using CashManager.Tests.ViewModels.Fixtures;

using Xunit;

namespace CashManager.Tests.ViewModels
{
    [Collection("Cleanable database collection")]
    public class StocksViewModelTests
    {
        private readonly CleanableDatabaseFixture _fixture;

        public StocksViewModelTests(CleanableDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void RemoveCommandExecute_FirstStock_FirstStockIsRemoved()
        {
            //given
            var startStocks = new[]
            {
                new Stock { Name = "A", IsUserStock = true },
                new Stock { Name = "B", IsUserStock = true },
                new Stock { Name = "C", IsUserStock = true },
            };
            var vm = _fixture.Container.Resolve<StocksViewModel>();
            vm.Update();
            vm.Stocks.AddRange(startStocks);
            var expectedStocks = vm.Stocks.Skip(1).OrderBy(x => x.Name).ToArray();

            //when
            vm.RemoveCommand.Execute(vm.Stocks[0]);

            //then
            Assert.Equal(expectedStocks, vm.Stocks.OrderBy(x => x.Name).ToArray());
        }
    }
}