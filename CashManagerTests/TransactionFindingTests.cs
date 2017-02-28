﻿using System;
using System.Collections.Generic;
using Logic;
using Logic.FindingFilters;
using Logic.Model;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

using NUnit.Framework;

namespace CashManagerTests
{
    [TestFixture]
    public class TransactionFindingTests
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
            targetStock = incomeSource = new Stock("Targetstock", 0);
            tempStock = new Stock("temp", 0);

            transactions = new Transactions();

            income = new Transaction(eTransactionType.Work, DateTime.Today, "Income", "Income today!");
            income.TargetStockId = mystock.Id;
            income.Subtransactions.Add(new Subtransaction("Payment income", INCOME_VALUE));
            income.TransactionSoucePayments.Add(new Logic.Model.TransactionPartPayment(incomeSource, INCOME_VALUE, ePaymentType.Value));

            transactions.Add(income);

            outcome = new Transaction(eTransactionType.Buy, DateTime.Today, "Buying sth", "");
            outcome.TargetStockId = targetStock.Id;

            Subtransaction foodSubtrans = new Subtransaction("Jedzenie", foodCost) { Category = new Category("Cat-Food") };
            outcome.Subtransactions.Add(foodSubtrans);
            Subtransaction drugSubtrans = new Subtransaction("Leki", drugCost) { Category = new Category("Cat-Drugs") };
            outcome.Subtransactions.Add(drugSubtrans);

            outcome.TransactionSoucePayments.Add(new Logic.Model.TransactionPartPayment(mystock, 100, ePaymentType.Percent));

            transactions.Add(outcome);
        }

        [Test]
        public void ShouldFindByTitle()
        {
            //given
            var searchCriteria  = new TitleContainsRule("incom");

            List<Transaction> _transactions = new List<Transaction>(transactions.TransactionsList);

            //when
            List<Transaction> found = _transactions.FindAll(t => searchCriteria.IsSatisfiedBy(t));

            //then
            Assert.Contains(income, found);
        }
    }
}
