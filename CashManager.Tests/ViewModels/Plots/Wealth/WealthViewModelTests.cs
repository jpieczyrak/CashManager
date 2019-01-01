using Autofac;

using CashManager_MVVM.Features.Plots;
using CashManager_MVVM.Model;

using OxyPlot;

using Xunit;

namespace CashManager.Tests.ViewModels.Plots.Wealth
{
    public class WealthViewModelTests : ViewModelTests
    {
        [Fact]
        public void GetWealthValues_NullNull_Empty()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<WealthViewModel>();
            var expected = new DataPoint[0];

            //when
            var result = vm.GetWealthValues(null, null);

            //then
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetWealthValues_EmptyEmpty_Empty()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<WealthViewModel>();
            var expected = new DataPoint[0];

            //when
            var result = vm.GetWealthValues(new Transaction[0], new Stock[0]);

            //then
            Assert.Equal(expected, result);
        }
    }
}