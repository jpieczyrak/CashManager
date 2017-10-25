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
            _targetStock = _incomeSource = new Stock("Targetstock");
            _tempStock = new Stock("temp");

            _transactions = new Transactions();

            _income = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!");
            _income.Subtransactions.Add(new Subtransaction("Payment _income", INCOME_VALUE));
            _income.Source = _incomeSource;
            _income.Target = _mystock;

            _transactions.Add(_income);

            _outcome = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "");

            var foodSubtrans = new Subtransaction("Jedzenie", foodCost) { Category = new Category("Cat-Food") };
            _outcome.Subtransactions.Add(foodSubtrans);
            var drugSubtrans = new Subtransaction("Leki", drugCost) { Category = new Category("Cat-Drugs") };
            _outcome.Subtransactions.Add(drugSubtrans);
            
            _outcome.Source = _mystock;
            _outcome.Target = _incomeSource;

            _transactions.Add(_outcome);
        }

        private const int INCOME_VALUE = 2000;
        private const int foodCost = 50;
        private const double drugCost = 21.45;
        private const double transactionCost = 165.43;

        private Stock _mystock;
        private Stock _incomeSource;
        private Stock _targetStock;
        private Stock _tempStock;

        private Transactions _transactions;

        private Transaction _income;
        private Transaction _outcome;

        [Test]
        public void ShouldFindByTitle()
        {
            //given
            var searchCriteria = new TitleContainsRule("incom");

            var transactions = new List<Transaction>(_transactions.TransactionsList);

            //when
            var found = transactions.FindAll(t => searchCriteria.IsSatisfiedBy(t));

            //then
            Assert.Contains(_income, found);
        }
    }
}