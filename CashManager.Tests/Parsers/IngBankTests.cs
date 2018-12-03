﻿using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Logic.Parsers;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class IngBankTests : BaseParserTests
    {
        [Fact]
        public void SimpleOutcome1ParseTest()
        {
            //given
            string input = @"

trash
Dziś 02.12.2018
Kategoria  Rozrywka i podróże
Opis
ICON FITNESS GLIWICE GLIWICE PL

Płatność kartą 02.12.2018 Nr karty 4246xx9261

Kwota
blokada:

-49,00 PLN

Konto
KONTO Direct

Saldo po transakcji
1 832,05 PLN

trash";
            var userStock = new Stock { Name = "Ing bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 12, 2);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            string title = @"ICON FITNESS GLIWICE GLIWICE PL Płatność kartą 02.12.2018 Nr karty 4246xx9261";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"Rozrywka i podróże, KONTO Direct saldo: {1832.05:#,##0.00} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { Value = 49.0d }
                    }
                }, userStock, externalStock, input);
            var parser = new IngBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, outcomeType, null).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
        }

        [Fact]
        public void SimpleOutcome2ParseTest()
        {
            //given
            string input = @"Wczoraj 01.12.2018,Kategoria Transfer wewnętrzny,Przelew własny,Kwota -110,00 PLN,Konto KONTO Direct,Saldo po transakcji 1 881,05 PLNrozwiń poniżej szczegóły
DataWczoraj 01.12.2018
Kategoria  Transfer wewnętrzny
Opis
Przelew własny

Kwota
-110,00 PLN

Konto
KONTO Direct

Saldo po transakcji
1 881,05 PLN";
            var userStock = new Stock { Name = "Ing bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 12, 1);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            string title = "Przelew własny";
            double balance = 1881.05;
            var expected = new Transaction(outcomeType, creationDate, title,
                $"Transfer wewnętrzny, KONTO Direct saldo: {balance:#,##0.00} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { Value = 110.0d }
                    }
                }, userStock, externalStock, input);
            var parser = new IngBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, outcomeType, null).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balance.Value);
        }

        [Fact]
        public void OutcomeParse_3Transactions_NoError3Transactions()
        {
            //given
            string input = @"

trash
Dziś 02.12.2018
Kategoria  Rozrywka i podróże
Opis
ICON FITNESS GLIWICE GLIWICE PL

Płatność kartą 02.12.2018 Nr karty 4246xx9261

Kwota
blokada:

-49,00 PLN

Konto
KONTO Direct

Saldo po transakcji
1 832,05 PLN

trash
asd


trash
Dziś 02.12.2018
Kategoria  Rozrywka i podróże
Opis
ICON FITNESS GLIWICE GLIWICE PL

Płatność kartą 02.12.2018 Nr karty 4246xx9261

Kwota
blokada:

-49,00 PLN

Konto
KONTO Direct

Saldo po transakcji
1 832,05 PLN

trash
trash

trash
Dziś 02.12.2018
Kategoria  Rozrywka i podróże
Opis
ICON FITNESS GLIWICE GLIWICE PL

Płatność kartą 02.12.2018 Nr karty 4246xx9261

Kwota
blokada:

-49,00 PLN

Konto
KONTO Direct

Saldo po transakcji
1 832,05 PLN

trash";
            var userStock = new Stock { Name = "Ing bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 12, 2);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            string title = @"ICON FITNESS GLIWICE GLIWICE PL Płatność kartą 02.12.2018 Nr karty 4246xx9261";
            double balance = 1832.05;
            var expected = new Transaction(outcomeType, creationDate, title,
                $"Rozrywka i podróże, KONTO Direct saldo: {balance:#,##0.00} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { Value = 49.0d }
                    }
                }, userStock, externalStock, input);
            var parser = new IngBankParser();

            //when
            var results = parser.Parse(input, userStock, externalStock, outcomeType, null);

            //then
            Assert.Equal(3, results.Length);
            foreach (var transaction in results) ValidateTransaction(transaction, expected);
            Assert.Equal(balance, parser.Balance.Value);
        }

        [Fact]
        public void SimpleIncomeParseTest()
        {
            //given
            string input = @"
trash
trash
Wczoraj 01.12.2018,Kategoria Przychód,NALICZONE ODSETKI,Kwota 2,63 PLN,Konto Smart Saver,Saldo po transakcji 1 479,68 PLNrozwiń poniżej szczegóły
DataWczoraj 01.12.2018
Kategoria  Przychód
Opis
NALICZONE ODSETKI

Kwota
2,63 PLN

Konto
Smart Saver

Saldo po transakcji
1 479,68 PLN
trash
";
            var userStock = new Stock { Name = "Ing bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 12, 1);
            var outcomeType = new TransactionType { Income = true, Name = "Work" };
            string title = @"NALICZONE ODSETKI";
            double balance = 1479.68;
            var expected = new Transaction(outcomeType, creationDate, title,
                $"Przychód, Smart Saver saldo: {balance:#,##0.00} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { Value = 2.63d }
                    }
                }, userStock, externalStock, input);
            var parser = new IngBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, outcomeType, null).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balance.Value);
        }
    }
}