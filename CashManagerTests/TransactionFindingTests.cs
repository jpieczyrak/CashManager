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
            _mystock = new Stock("Mystock", 0);
            _targetStock = _incomeSource = new Stock("Targetstock", 0);
            _tempStock = new Stock("temp", 0);

            _transactions = new Transactions();

            _income = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!");
            _income.TargetStockId = _mystock.Id;
            _income.Subtransactions.Add(new Subtransaction("Payment _income", INCOME_VALUE));
            _income.TransactionSoucePayments.Add(new TransactionPartPayment(_incomeSource, INCOME_VALUE, ePaymentType.Value));

            _transactions.Add(_income);

            _outcome = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "");
            _outcome.TargetStockId = _targetStock.Id;

            var foodSubtrans = new Subtransaction("Jedzenie", foodCost) { Category = new Category("Cat-Food") };
            _outcome.Subtransactions.Add(foodSubtrans);
            var drugSubtrans = new Subtransaction("Leki", drugCost) { Category = new Category("Cat-Drugs") };
            _outcome.Subtransactions.Add(drugSubtrans);

            _outcome.TransactionSoucePayments.Add(new TransactionPartPayment(_mystock, 100, ePaymentType.Percent));

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