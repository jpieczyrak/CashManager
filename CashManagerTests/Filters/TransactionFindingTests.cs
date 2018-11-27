using System;
using System.Collections.Generic;

using LogicOld.FindingFilters;
using LogicOld.Model;
using LogicOld.TransactionManagement.TransactionElements;

using NUnit.Framework;

namespace CashManagerTests.Filters
{
    [TestFixture]
    public class TransactionFindingTests
    {
        [SetUp]
        public void Init()
        {
            _mystock = new Stock("Mystock");

            _incomeTransaction = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!",
                new List<Position> { new Position("Payment _income", INCOME_VALUE) }, 
                _mystock, _externalStock, "1st input");
            
            var foodSubtrans = new Position("Jedzenie", foodCost) { Category = new Category("Cat-Food") };
            var drugSubtrans = new Position("Leki", drugCost) { Category = new Category("Cat-Drugs") };

            _outcomeTransaction = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "", 
                new List<Position> {foodSubtrans, drugSubtrans}, 
                _mystock, _externalStock, "2nd input");
        }

        private const int INCOME_VALUE = 2000;
        private const int foodCost = 50;
        private const double drugCost = 21.45;

        private Stock _mystock;
        private Stock _externalStock;
        
        private Transaction _incomeTransaction;
        private Transaction _outcomeTransaction;

        [Test]
        public void ShouldFindByTitle()
        {
            //given
            var searchCriteria = new TitleContainsRule("incom");

            var transactions = new List<Transaction> {_incomeTransaction, _outcomeTransaction};

            //when
            var found = transactions.FindAll(t => searchCriteria.IsSatisfiedBy(t));

            //then
            Assert.Contains(_incomeTransaction, found);
        }
    }
}