using System;
using System.Linq;

using CashManager.Data.Extensions;
using CashManager.Logic.Calculators;
using CashManager.Model;
using CashManager.Model.Selectors;

using OxyPlot;
using OxyPlot.Axes;

using Xunit;

namespace CashManager.Tests.MVVM.Logic
{
    public class TransactionBalanceCalculatorTests
    {
        private static readonly Transaction[] _transactions;
        private static readonly Stock[] _selectedStocks;
        private static readonly DateFrameSelector _dateFilter;
        private static DateTime _firstBookDate;

        static TransactionBalanceCalculatorTests()
        {
            var notSelectedStock = new Stock("ns".GenerateGuid());
            _selectedStocks = new[] { new Stock("selected".GenerateGuid()) { IsUserStock = true } };
            var selectedUserStock = _selectedStocks[0];
            selectedUserStock.IsPropertyChangedEnabled = true;
            selectedUserStock.Balance.IsPropertyChangedEnabled = true;
            selectedUserStock.Balance.Value = 60000m;
            var income = new TransactionType { Income = true };
            var outcome = new TransactionType { Outcome = true };
            _firstBookDate = DateTime.Today.AddDays(-30);
            _dateFilter = new DateFrameSelector(DateFrameType.BookDate);
            _transactions = new[]
            {
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-10),
                    UserStock = selectedUserStock,
                    Type = outcome,
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(1000, 1000, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-12),
                    UserStock = notSelectedStock,
                    Type = outcome,
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(100, 100, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = DateTime.Today.AddDays(-22),
                    UserStock = selectedUserStock,
                    Type = outcome,
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(10, 10, 0) }
                    }
                },
                new Transaction
                {
                    BookDate = _firstBookDate,
                    UserStock = selectedUserStock,
                    Type = income,
                    Positions = new TrulyObservableCollection<Position>
                    {
                        new Position { Value = new PaymentValue(10000, 10000, 0) }
                    }
                },
            };
        }

        [Fact]
        public void GetWealthValues_SomeTransactions_Matching()
        {
            //given
            var calculator = new TransactionBalanceCalculator();

            var expected = new[]
            {
                new DataPoint(DateTimeAxis.ToDouble(_firstBookDate.AddDays(-1)), 51010.0d),
                new DataPoint(DateTimeAxis.ToDouble(_firstBookDate), 61010.0d),
                new DataPoint(DateTimeAxis.ToDouble(DateTime.Today.AddDays(-22)), 61000.0d),
                new DataPoint(DateTimeAxis.ToDouble(DateTime.Today.AddDays(-10)), 60000.0d),
                new DataPoint(DateTimeAxis.ToDouble(DateTime.Today), 60000.0d),
            };

            //when
            var result = calculator.GetWealthValues(_transactions, _selectedStocks, _dateFilter, x => x.BookDate, false);

            //then
            Assert.Equal(expected.Length, result.Length);
            Assert.Equal(expected, result);
        }
        [Fact]
        public void CalculateBalance_SomeTransactions_Matching()
        {
            //given
            var calculator = new TransactionBalanceCalculator();

            var expected = new[]
            {
                new TransactionBalance(_firstBookDate.AddDays(-1), 51010m),
                new TransactionBalance(_firstBookDate, 61010m),
                new TransactionBalance(DateTime.Today.AddDays(-22), 61000m),
                new TransactionBalance(DateTime.Today.AddDays(-10), 60000m),
                new TransactionBalance(DateTime.Today, 60000m),
            };

            //when
            var result = calculator.CalculateBalance(_transactions, _selectedStocks, _dateFilter, x => x.BookDate, false)
                                   .OrderBy(x => x.BookDate)
                                   .ToArray();

            //then
            Assert.Equal(expected.Length, result.Length);
            Assert.Equal(expected, result);
        }
    }
}