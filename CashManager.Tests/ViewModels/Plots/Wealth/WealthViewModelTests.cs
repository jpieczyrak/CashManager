﻿using System;

using Autofac;

using CashManager.Data.Extensions;
using CashManager.Features.Plots;
using CashManager.Model;
using CashManager.Tests.ViewModels.Fixtures;

using OxyPlot;
using OxyPlot.Axes;

using Xunit;

namespace CashManager.Tests.ViewModels.Plots.Wealth
{
    [Collection("Cleanable database collection")]
    public class WealthViewModelTests
    {
        private readonly CleanableDatabaseFixture _fixture;

        public WealthViewModelTests(CleanableDatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.Reset();
        }

        [Fact]
        public void GetWealthValues_NullNull_Empty()
        {
            //given
            var vm = _fixture.Container.Resolve<WealthViewModel>();
            var expected = new DataPoint[0];

            //when
            var result = vm.GetWealthValues(null, null, false);

            //then
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetWealthValues_EmptyEmpty_Empty()
        {
            //given
            var vm = _fixture.Container.Resolve<WealthViewModel>();
            var expected = new DataPoint[0];

            //when
            var result = vm.GetWealthValues(new Transaction[0], new Stock[0], false);

            //then
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetWealthValues_NonEmpty_Matching()
        {
            //given
            var vm = _fixture.Container.Resolve<WealthViewModel>();
            var notSelectedStock = new Stock("ns".GenerateGuid());
            var selectedStocks = new[] { new Stock("selected".GenerateGuid()) { IsUserStock = true } };
            var selectedUserStock = selectedStocks[0];
            selectedUserStock.IsPropertyChangedEnabled = true;
            selectedUserStock.Balance.IsPropertyChangedEnabled = true;
            selectedUserStock.Balance.Value = 60000m;
            var firstBookDate = DateTime.Today.AddDays(-30);
            var income = new TransactionType { Income = true };
            var outcome = new TransactionType { Outcome = true };
            var transactions = new []
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
                    BookDate = firstBookDate,
                    UserStock = selectedUserStock,
                    Type = income,
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
                new DataPoint(DateTimeAxis.ToDouble(DateTime.Today), 60000.0d)
            };

            //when
            var result = vm.GetWealthValues(transactions, selectedStocks, false);

            //then
            Assert.Equal(expected.Length, result.Length);
            Assert.Equal(expected, result);
        }
    }
}