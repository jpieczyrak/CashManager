using System;
using System.Linq;

using Autofac;

using CashManager_MVVM;
using CashManager_MVVM.Features.Plots;
using CashManager_MVVM.Model;

using OxyPlot;
using OxyPlot.Axes;

using Xunit;

namespace CashManager.Tests.ViewModels.Plots.Wealth
{
    public class WealthViewModelTests : ViewModelTests
    {
        [Fact]
        public void GetWealthValues_NullNull_Empty()
        {
            //given
            var vm = Container.Resolve<WealthViewModel>();
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
            var vm = Container.Resolve<WealthViewModel>();
            var expected = new DataPoint[0];

            //when
            var result = vm.GetWealthValues(new Transaction[0], new Stock[0]);

            //then
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetWealthValues_NonEmpty_Matching()
        {
            //given
            var vm = Container.Resolve<WealthViewModel>();
            var selectedStocks = Stocks.Value.Take(1).ToArray();
            var selectedUserStock = Stocks.Value[0];
            var firstBookDate = DateTime.Today.AddDays(-30);
            var transactions = new []
            {
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-10),
                    UserStock = selectedUserStock,
                    Type = Types.Value[1],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(1000, 1000, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-12),
                    UserStock = Stocks.Value[1],
                    Type = Types.Value[1],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(100, 100, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-22),
                    UserStock = selectedUserStock,
                    Type = Types.Value[1],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(10, 10, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = firstBookDate,
                    UserStock = selectedUserStock,
                    Type = Types.Value[0],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(10000, 10000, 0) }
                    }
                },
            };
            var expected = new[]
            {
                new DataPoint(DateTimeAxis.ToDouble(firstBookDate.AddDays(-1)), 51010.0d), 
                new DataPoint(DateTimeAxis.ToDouble(firstBookDate), 61010.0d), 
                new DataPoint(DateTimeAxis.ToDouble(DateTime.Today.AddDays(-22)), 61000.0d), 
                new DataPoint(DateTimeAxis.ToDouble(DateTime.Today.AddDays(-10)), 60000.0d), 
                new DataPoint(DateTimeAxis.ToDouble(DateTime.Today), 60000.0d), 
            };

            //when
            var result = vm.GetWealthValues(transactions, selectedStocks);

            //then
            Assert.Equal(expected.Length, result.Length);
            Assert.Equal(expected, result);
        }
    }
}