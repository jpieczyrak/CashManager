﻿using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Logic.Parsers;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class MillenniumBankTests : BaseParserTests
    {
        [Fact]
        public void SimpleOutcomeParseTest()
        {
            //given
            string input = @"
some trash

Zaznacz
	
	30-11-2018 / 30-11-2018 	
Typ: TRANSAKCJA KARTĄ PŁATNICZĄ
Tytuł: Centrum Niskich Cen Sp.Gliwice
	-35,80 PLN
Saldo: 1 253,76 PLN
	

    Szczegóły transakcji";
            var userStock = new Stock { Name = "Millennium bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 11, 30);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            string title = "Centrum Niskich Cen Sp.Gliwice";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"TRANSAKCJA KARTĄ PŁATNICZĄ, Saldo: {1253.76:#,##0.00}",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { Value = 35.8d }
                    }
                }, userStock, externalStock, input);
            var parser = new MillenniumBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, outcomeType, null).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
        }

        [Fact]
        public void OutcomeParse_2Transactions_NoError2Transactions()
        {
            //given
            string input = @"

Zaznacz
	
	30-11-2018 / 30-11-2018 	
Typ: TRANSAKCJA KARTĄ PŁATNICZĄ
Tytuł: Centrum Niskich Cen Sp.Gliwice
	-35,80 PLN
Saldo: 1 253,76 PLN
	

    Szczegóły transakcji

Zaznacz
	
	30-11-2018 / 30-11-2018 	
Typ: TRANSAKCJA KARTĄ PŁATNICZĄ
Tytuł: Centrum Niskich Cen Sp.Gliwice
	-35,80 PLN
Saldo: 1 253,76 PLN
	

    Szczegóły transakcji
	

spam not valid trash";
            var userStock = new Stock { Name = "Millennium bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 11, 30);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            string title = "Centrum Niskich Cen Sp.Gliwice";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"TRANSAKCJA KARTĄ PŁATNICZĄ, Saldo: {1253.76:#,##0.00}",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { Value = 35.8d }
                    }
                }, userStock, externalStock, input);
            var parser = new MillenniumBankParser();

            //when
            var results = parser.Parse(input, userStock, externalStock, outcomeType, null);

            //then
            Assert.Equal(2, results.Count);
            foreach (var transaction in results) ValidateTransaction(transaction, expected);
        }

        [Fact]
        public void SimpleIncomeParseTest()
        {
            //given
            string input = @"Zaznacz
	 
	26-11-2018 / 26-11-2018 	
Typ: PRZELEW PRZYCHODZĄCY 
Tytuł: Za kwiatki
	13,50 PLN
Saldo: 1 253,76 PLN
	

    Szczegóły transakcji";
            var userStock = new Stock { Name = "Millennium bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 11, 26);
            var outcomeType = new TransactionType { Income = true, Name = "Income" };
            string title = "Za kwiatki";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"PRZELEW PRZYCHODZĄCY, Saldo: {1253.76:#,##0.00}",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { Value = 13.5d }
                    }
                }, userStock, externalStock, input);
            var parser = new MillenniumBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, outcomeType, null).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
        }
    }
}