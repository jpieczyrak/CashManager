﻿using System;
using System.Collections.Generic;
using System.Linq;

using Logic.IoC;
using Logic.LogicObjectsProviders;
using Logic.Mapping;
using Logic.Model;
using Logic.Parsing;
using Logic.TransactionManagement.TransactionElements;

using NUnit.Framework;

namespace CashManagerTests.Parsing
{
    [TestFixture]
    public class GetinBankParserTests
    {
        [SetUp]
        public void Setup()
        {
            MapperConfiguration.Configure();
            AutofacConfiguration.UseStrategy(eBuildStrategyType.Test);
        }

        [Test]
        public void OutcomeParseTest()
        {
            //given
            string input = @"    06.09.2016 – PRZELEW WYCHODZĄCY
JĘDRZEJ PIECZYRAK – [Sierpień] Czynsz + media

-684,62 PLN";

            var parser = new GetinBankParser();

            var userStock = StockProvider.AddNew("Getin");
            var externalStock = StockProvider.Default;
            var creationDate = new DateTime(2016, 9, 6);
            var expected = new Transaction(eTransactionType.Buy, creationDate, "[Sierpień] Czynsz + media",
                "JĘDRZEJ PIECZYRAK: PRZELEW WYCHODZĄCY (PLN)",
                new List<Subtransaction>
                {
                    new Subtransaction("[Sierpień] Czynsz + media", 684.62d)
                }, userStock, externalStock, input);


            //when
            var output = parser.Parse(input, userStock);
            var parsed = output.FirstOrDefault();

            //then
            Assert.NotNull(parsed);
            Assert.AreEqual(expected.BookDate, parsed.BookDate);
            Assert.AreEqual(expected.Title, parsed.Title);
            Assert.AreEqual(expected.Note, parsed.Note);
            Assert.AreEqual(expected.MyStock, parsed.MyStock);
            Assert.AreEqual(expected.ExternalStock, parsed.ExternalStock);

            Assert.AreEqual(creationDate, parsed.BookDate);
            Assert.AreEqual(creationDate, parsed.TransationSourceCreationDate);

            Assert.AreEqual(expected.InstanceCreationDate, parsed.InstanceCreationDate);
            
            Assert.AreEqual(expected.Subtransactions.First().Name, parsed.Subtransactions.First().Name);
            Assert.AreEqual(expected.Subtransactions.First().Value, parsed.Subtransactions.First().Value);

            Assert.AreEqual(expected.Value, parsed.Value);
        }

        [Test]
        public void IncomeParseTest()
        {
            //given
            string input = @"28.02.2014 – PRZELEW PRZYCHODZĄCY
Firma SP. Z O.O. – Wynagrodzenie z tytulu umowy cywilnoprawnej

+1 123,12 PLN saldo po operacji: 1 574,38 PLN";

            var parser = new GetinBankParser();

            var userStock = StockProvider.AddNew("Getin");
            var externalStock = StockProvider.Default;
            var creationDate = new DateTime(2014, 02, 28);
            var expected = new Transaction(eTransactionType.Work, creationDate, "Wynagrodzenie z tytulu umowy cywilnoprawnej",
                "Firma SP. Z O.O.: PRZELEW PRZYCHODZĄCY (PLN)",
                new List<Subtransaction>
                {
                    new Subtransaction("Wynagrodzenie z tytulu umowy cywilnoprawnej", 1123.12d)
                }, userStock, externalStock, input);

            //when
            var output = parser.Parse(input, userStock);
            var parsed = output.FirstOrDefault();

            //then
            Assert.NotNull(parsed);
            Assert.AreEqual(expected.BookDate, parsed.BookDate);
            Assert.AreEqual(expected.Title, parsed.Title);
            Assert.AreEqual(expected.Note, parsed.Note);
            Assert.AreEqual(expected.MyStock, parsed.MyStock);
            Assert.AreEqual(expected.ExternalStock, parsed.ExternalStock);

            Assert.AreEqual(creationDate, parsed.BookDate);
            Assert.AreEqual(creationDate, parsed.TransationSourceCreationDate);

            Assert.AreEqual(expected.Subtransactions.First().Name, parsed.Subtransactions.First().Name);
            Assert.AreEqual(expected.Subtransactions.First().Value, parsed.Subtransactions.First().Value);

            Assert.AreEqual(expected.Value, parsed.Value);
        }
    }
}