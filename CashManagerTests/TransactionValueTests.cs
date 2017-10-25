using System;

using Logic.LogicObjectsProviders;
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
            _targetStock = _incomeSource = new Stock("Targetstock");
            _tempStock = new Stock("temp");

            _transactions = new Transactions();

            _income = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!") { TargetStockId = Guid.Empty };
            _income.Subtransactions.Add(new Subtransaction("Payment _income", INCOME_VALUE));
            _income.TransactionSoucePayments.Add(new TransactionPartPayment(_incomeSource, INCOME_VALUE, ePaymentType.Value));

            _transactions.Add(_income);

            _outcome = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "") { TargetStockId = Guid.Empty };

            var foodSubtrans = new Subtransaction("Jedzenie", FOOD_COST) { Category = new Category("Cat-Food") };
            _outcome.Subtransactions.Add(foodSubtrans);
            var drugSubtrans = new Subtransaction("Leki", DRUG_COST) { Category = new Category("Cat-Drugs") };
            _outcome.Subtransactions.Add(drugSubtrans);

            _outcome.TransactionSoucePayments.Add(new TransactionPartPayment(_mystock, 100, ePaymentType.Percent));

            _transactions.Add(_outcome);
        }

        private const int INCOME_VALUE = 2000;
        private const int FOOD_COST = 50;
        private const double DRUG_COST = 21.45;
        private const double TRANSACTION_COST = 165.43;

        private Stock _mystock;
        private Stock _incomeSource;
        private Stock _targetStock;
        private Stock _tempStock;

        private Transactions _transactions;

        private Transaction _income;
        private Transaction _outcome;

        [TestCase(eTransactionType.Buy, TRANSACTION_COST, ePaymentType.Value, -TRANSACTION_COST)]
        [TestCase(eTransactionType.Reinvest, TRANSACTION_COST, ePaymentType.Value, -TRANSACTION_COST)]
        [TestCase(eTransactionType.Work, TRANSACTION_COST, ePaymentType.Value, TRANSACTION_COST)]
        [TestCase(eTransactionType.Sell, TRANSACTION_COST, ePaymentType.Value, TRANSACTION_COST)]
        [TestCase(eTransactionType.Resell, TRANSACTION_COST, ePaymentType.Value, TRANSACTION_COST)]
        [TestCase(eTransactionType.Transfer, TRANSACTION_COST, ePaymentType.Value, 0)]
        [TestCase(eTransactionType.Buy, TRANSACTION_COST, ePaymentType.Percent, -TRANSACTION_COST)]
        [TestCase(eTransactionType.Reinvest, TRANSACTION_COST, ePaymentType.Percent, -TRANSACTION_COST)]
        [TestCase(eTransactionType.Work, TRANSACTION_COST, ePaymentType.Percent, TRANSACTION_COST)]
        [TestCase(eTransactionType.Sell, TRANSACTION_COST, ePaymentType.Percent, TRANSACTION_COST)]
        [TestCase(eTransactionType.Resell, TRANSACTION_COST, ePaymentType.Percent, TRANSACTION_COST)]
        [TestCase(eTransactionType.Transfer, TRANSACTION_COST, ePaymentType.Percent, 0)]
        public void ShouldShowProperValueWithSign(eTransactionType type, double value, ePaymentType payment, double expected)
        {
            //given
            var transaction = new Transaction(type, DateTime.Now, "title", "note") { TargetStockId = Guid.Empty };

            //2 subtransactions
            transaction.Subtransactions.Add(new Subtransaction("test1", value / 2));
            transaction.Subtransactions.Add(new Subtransaction("test2", value / 2));

            //2 sources
            transaction.TransactionSoucePayments.Add(new TransactionPartPayment(_tempStock,
                payment == ePaymentType.Value ? value * 0.75 : 75, payment));
            transaction.TransactionSoucePayments.Add(new TransactionPartPayment(_tempStock,
                payment == ePaymentType.Value ? value * 0.25 : 25, payment));

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