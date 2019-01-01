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

        [Fact]
        public void GetWealthValues_NonEmpty_Matching()
        {
            //given
            SetupDatabase();
            var vm = _container.Resolve<WealthViewModel>();
            var selectedStocks = Stocks.Take(1).ToArray();
            var selectedUserStock = Stocks[0];
            var transactions = new []
            {
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-10),
                    UserStock = selectedUserStock,
                    Type = Types[1],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(1000, 1000, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-12),
                    UserStock = Stocks[1],
                    Type = Types[1],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(100, 100, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-22),
                    UserStock = selectedUserStock,
                    Type = Types[1],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(10, 10, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-30),
                    UserStock = selectedUserStock,
                    Type = Types[0],
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(10000, 10000, 0) }
                    }
                },
            };
            decimal startValue = selectedUserStock.UserBalance;
            var expected = transactions
                           .Where(x => selectedStocks.Contains(x.UserStock))
                           .OrderByDescending(x => x.BookDate)
                           .GroupBy(x => x.BookDate.Date)
                           .Select(x => new { BookDate = x.Key, Value = (startValue -= x.Sum(y => y.ValueAsProfit)) } )
                           .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.BookDate), (double)x.Value))
                           .Concat(new[] 
                           {
                               new DataPoint(DateTimeAxis.ToDouble(selectedUserStock.LastEditDate), 
                               (double) selectedUserStock.UserBalance)
                           })
                           .OrderBy(x => x.X)
                           .ToArray();

            //when
            var result = vm.GetWealthValues(transactions, selectedStocks);

            //then
            Assert.Equal(expected.Length, result.Length);
            Assert.Equal(expected, result);
        }
    }
}