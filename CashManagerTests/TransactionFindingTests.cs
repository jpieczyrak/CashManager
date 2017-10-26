using System;
using System.Collections.Generic;

using Logic.FindingFilters;
using Logic.Model;
using Logic.TransactionManagement.TransactionElements;

using NUnit.Framework;

namespace CashManagerTests
{
    [TestFixture]
    public class TransactionFindingTests
    {
        [SetUp]
        public void Init()
        {
            _mystock = new Stock("Mystock");

            _incomeTransaction = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!",
                new List<Subtransaction> { new Subtransaction("Payment _income", INCOME_VALUE) }, 
                _mystock, _externalStock);
            
            var foodSubtrans = new Subtransaction("Jedzenie", foodCost) { Category = new Category("Cat-Food") };
            var drugSubtrans = new Subtransaction("Leki", drugCost) { Category = new Category("Cat-Drugs") };

            _outcomeTransaction = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "", 
                new List<Subtransaction> {foodSubtrans, drugSubtrans}, 
                _mystock, _externalStock);
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