using System;
using System.ComponentModel.Design;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using NUnit.Framework;

namespace CashManagerTests
{
    [TestFixture]
    public class TransactionValueTests
    {
        private const int INCOME_VALUE = 2000;
        private const int foodCost = 50;
        private const double drugCost = 21.45;
        private const double transactionCost = 165.43;

        Stock mystock;
        Stock incomeSource;
        Stock targetStock;
        Stock tempStock;

        Transactions transactions;

        Transaction income;
        Transaction outcome;
        

        [SetUp]
        public void Init()
        {
            mystock = new Stock("Mystock", 0);
            incomeSource = new Stock("Targetstock", 0);
            tempStock = new Stock("temp", 0);

            transactions = new Transactions();

            income = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!");
            income.TargetStock = mystock;
            income.Subtransactions.Add(new Subtransaction("Payment income", INCOME_VALUE));
            income.TransactionSoucePayments.Add(new TransactionPartPayment(incomeSource, INCOME_VALUE, ePaymentType.Value));

            transactions.Add(income);

            outcome = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "");
            outcome.TargetStock = targetStock;

            Subtransaction foodSubtrans = new Subtransaction("Jedzenie", foodCost) {Category = "Cat-Food"};
            outcome.Subtransactions.Add(foodSubtrans);
            Subtransaction drugSubtrans = new Subtransaction("Leki", drugCost) { Category = "Cat-Drugs" };
            outcome.Subtransactions.Add(drugSubtrans);

            outcome.TransactionSoucePayments.Add(new TransactionPartPayment(mystock, 100, ePaymentType.Percent));

            transactions.Add(outcome);
        }

        [Test]
        public void ShouldProperCalculateTransactionValue()
        {
            //given
            double expectedCost = foodCost + drugCost;

            //when
            double actualCost = outcome.Value;

            //then
            Assert.AreEqual(expectedCost, actualCost);
        }

        [TestCase(eTransactionType.Buy, transactionCost, ePaymentType.Value, -transactionCost)]
        [TestCase(eTransactionType.Reinvest, transactionCost, ePaymentType.Value, -transactionCost)]
        [TestCase(eTransactionType.Work, transactionCost, ePaymentType.Value, transactionCost)]
        [TestCase(eTransactionType.Sell, transactionCost, ePaymentType.Value, transactionCost)]
        [TestCase(eTransactionType.Resell, transactionCost, ePaymentType.Value, transactionCost)]
        [TestCase(eTransactionType.Transfer, transactionCost, ePaymentType.Value, 0)]
        [TestCase(eTransactionType.Buy, transactionCost, ePaymentType.Percent, -transactionCost)]
        [TestCase(eTransactionType.Reinvest, transactionCost, ePaymentType.Percent, -transactionCost)]
        [TestCase(eTransactionType.Work, transactionCost, ePaymentType.Percent, transactionCost)]
        [TestCase(eTransactionType.Sell, transactionCost, ePaymentType.Percent, transactionCost)]
        [TestCase(eTransactionType.Resell, transactionCost, ePaymentType.Percent, transactionCost)]
        [TestCase(eTransactionType.Transfer, transactionCost, ePaymentType.Percent, 0)]
        public void ShouldShowProperValueWithSign(eTransactionType type, double value, ePaymentType payment, double expected)
        {
            //given
            Transaction transaction = new Transaction(type, DateTime.Now, "title", "note");
            transaction.TargetStock = Stock.Unknown;

            //2 subtransactions
            transaction.Subtransactions.Add(new Subtransaction("test1", value/2));
            transaction.Subtransactions.Add(new Subtransaction("test2", value/2));

            //2 sources
            transaction.TransactionSoucePayments.Add(new TransactionPartPayment(tempStock, payment == ePaymentType.Value ? value * 0.75 : 75, payment));
            transaction.TransactionSoucePayments.Add(new TransactionPartPayment(tempStock, payment == ePaymentType.Value ? value * 0.25 : 25, payment));
            
            //when
            double actualValue = transaction.ValueAsProfit;

            //then
            Assert.AreEqual(expected, actualValue);
        }
    }
}