﻿using System;

using Logic.Model;
using Logic.TransactionManagement.TransactionElements;

using NUnit.Framework;

namespace CashManagerTests
{
    [TestFixture]
    public class TransactionValueTests
    {
        [SetUp]
        public void Init()
        {
            _mystock = new Stock("Mystock");

            _transactions = new Transactions();

            _income = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!");
            _income.Subtransactions.Add(new Subtransaction("Payment _income", INCOME_VALUE));
            _income.MyStock = _incomeSource;
            _income.ExternalStock = _mystock;

            _transactions.Add(_income);

            _outcome = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "");

            var foodSubtrans = new Subtransaction("Jedzenie", FOOD_COST) { Category = new Category("Cat-Food") };
            _outcome.Subtransactions.Add(foodSubtrans);
            var drugSubtrans = new Subtransaction("Leki", DRUG_COST) { Category = new Category("Cat-Drugs") };
            _outcome.Subtransactions.Add(drugSubtrans);
            
            _outcome.MyStock = _mystock;
            _outcome.ExternalStock = _incomeSource;

            _transactions.Add(_outcome);
        }

        private const int INCOME_VALUE = 2000;
        private const int FOOD_COST = 50;
        private const double DRUG_COST = 21.45;
        private const double TRANSACTION_COST = 165.43;

        private Stock _mystock;
        private Stock _incomeSource;

        private Transactions _transactions;

        private Transaction _income;
        private Transaction _outcome;

        [TestCase(eTransactionType.Buy, TRANSACTION_COST, -TRANSACTION_COST)]
        [TestCase(eTransactionType.Reinvest, TRANSACTION_COST, -TRANSACTION_COST)]
        [TestCase(eTransactionType.Work, TRANSACTION_COST, TRANSACTION_COST)]
        [TestCase(eTransactionType.Sell, TRANSACTION_COST, TRANSACTION_COST)]
        [TestCase(eTransactionType.Resell, TRANSACTION_COST, TRANSACTION_COST)]
        [TestCase(eTransactionType.Transfer, TRANSACTION_COST, 0)]
        public void ShouldShowProperValueWithSign(eTransactionType type, double value, double expected)
        {
            //given
            var transaction = new Transaction(type, DateTime.Now, "title", "note");

            //2 subtransactions
            transaction.Subtransactions.Add(new Subtransaction("test1", value / 2));
            transaction.Subtransactions.Add(new Subtransaction("test2", value / 2));
            
            //when
            double actualValue = transaction.ValueAsProfit;

            //then
            Assert.AreEqual(expected, actualValue);
        }

        [Test]
        public void ShouldProperCalculateTransactionValue()
        {
            //given
            double expectedCost = FOOD_COST + DRUG_COST;

            //when
            double actualCost = _outcome.Value;

            //then
            Assert.AreEqual(expectedCost, actualCost);
        }
    }
}