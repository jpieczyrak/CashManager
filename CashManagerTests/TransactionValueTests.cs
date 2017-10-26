using System;
using System.Collections.Generic;

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
            _externalStock = new Stock("someone");
            
            var foodSubtrans = new Subtransaction("Jedzenie", FOOD_COST) { Category = new Category("Cat-Food") };
            var drugSubtrans = new Subtransaction("Leki", DRUG_COST) { Category = new Category("Cat-Drugs") };
            
            _outcome = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "", new List<Subtransaction> {foodSubtrans, drugSubtrans}, _mystock, _externalStock, "input");

        }
        
        private const int FOOD_COST = 50;
        private const double DRUG_COST = 21.45;
        private const double TRANSACTION_COST = 165.43;

        private Stock _mystock;
        private Stock _externalStock;
        
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
            
            var subtransactions = new List<Subtransaction>
            {
                new Subtransaction("test1", value / 2),
                new Subtransaction("test2", value / 2)
            };
            var transaction = new Transaction(type, DateTime.Now, "title", "note", subtransactions, _mystock, _externalStock, "input");

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