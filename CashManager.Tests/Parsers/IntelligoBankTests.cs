using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Logic.Parsers;
using CashManager.Properties;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class IntelligoBankTests : BaseParserTests
    {
        [Fact]
        public void SimpleOutcome1ParseTest()
        {
            //given
            string input = @"
trash

200 	2018-12-04 	2018-12-02 	Płatność kartą 	-12,34 	PLN 	9 876,60 	Lokalizacja:
Kraj: POLSKA
Miasto: GLIWICE
Adres: ADRES
Data wykonania: 2018-12-02 00:00:00
Numer referencyjny: 12312312312312312313123
Oryginalna kwota operacji: 12.34 PLN
Numer karty: * *123
Data waluty: 2018-12-02 
trash";
            var userStock = new Stock { Name = "Intelligo bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 12, 2);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            string title = @"200 Płatność kartą";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"200 - Lokalizacja: Kraj: POLSKA Miasto: GLIWICE Adres: ADRES Data wykonania: 2018-12-02 00:00:00 Numer referencyjny: 12312312312312312313123 Oryginalna kwota operacji: 12.34 PLN Numer karty: * *123 Data waluty: 2018-12-02 Płatność kartą saldo: {9876.6.ToString(Strings.ValueFormat)} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 12.34m }
                    }
                }, userStock, externalStock);
            var parser = new IntelligoBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, outcomeType, null).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
        }

        [Fact]
        public void SimpleOutcome2ParseTest()
        {
            //given
            string input = @"
trash

200 	2018-12-04 	2018-12-02 	Płatność kartą 	-12,34 	PLN 	9 876,60 	Lokalizacja:
Kraj: POLSKA
Miasto: GLIWICE
Adres: ADRES
Data wykonania: 2018-12-02 00:00:00
Numer referencyjny: 12312312312312312313123
Oryginalna kwota operacji: 12.34 PLN
Numer karty: * *123
Data waluty: 2018-12-02 
trash
200 	2018-12-04 	2018-12-02 	Płatność kartą 	-12,34 	PLN 	9 876,60 	Lokalizacja:
Kraj: POLSKA
Miasto: GLIWICE
Adres: ADRES
Data wykonania: 2018-12-02 00:00:00
Numer referencyjny: 12312312312312312313123
Oryginalna kwota operacji: 12.34 PLN
Numer karty: * *123
Data waluty: 2018-12-02 
trash";
            var userStock = new Stock { Name = "Intelligo bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 12, 2);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            decimal balance = 9876.6m;
            string title = @"200 Płatność kartą";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"200 - Lokalizacja: Kraj: POLSKA Miasto: GLIWICE Adres: ADRES Data wykonania: 2018-12-02 00:00:00 Numer referencyjny: 12312312312312312313123 Oryginalna kwota operacji: 12.34 PLN Numer karty: * *123 Data waluty: 2018-12-02 Płatność kartą saldo: {9876.6.ToString(Strings.ValueFormat)} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 12.34m }
                    }
                }, userStock, externalStock);
            var parser = new IntelligoBankParser();

            //when
            var results = parser.Parse(input, userStock, externalStock, outcomeType, null);

            //then
            foreach (var result in results) ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balances.First().Value.Value);
        }

        [Fact]
        public void SimpleIncomeParseTest()
        {
            //given
            string input = @"
spam trash 123

201 	2018-12-06 	2018-12-06 	Przelew na rachunek 	+1 000,05 	PLN 	10 876,65 	Nr rach. przeciwst.:
00 1111 2222 3333 4444 5555 6666
Dane adr. rach. przeciwst.:
SUPER FIRMA
SP. Z O.O.
UL. imię nazwisko 321
44-100 GLIWICE
Tytuł: Wyplata: ETAT/2018/11/abc
Data waluty: 2018-12-06 

temp trash
11";
            var userStock = new Stock { Name = "Intelligo bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 12, 6);
            var incomeType = new TransactionType { Income = true, Name = "Work" };
            string title = @"Wyplata: ETAT/2018/11/abc";
            decimal balance = 10876.65m;
            var expected = new Transaction(incomeType, creationDate, title,
                $"201 - Nr rach. przeciwst.: 00 1111 2222 3333 4444 5555 6666 Dane adr. rach. przeciwst.: SUPER FIRMA SP. Z O.O. UL. imię nazwisko 321 44-100 GLIWICE Tytuł: Wyplata: ETAT/2018/11/abc Data waluty: 2018-12-06 Przelew na rachunek saldo: {balance.ToString(Strings.ValueFormat)} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 1000.05m }
                    }
                }, userStock, externalStock);
            var parser = new IntelligoBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, null, incomeType).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balances.First().Value.Value);
        }

        [Fact]
        public void ShortUncommonOutcomeParseTest()
        {
            //given
            string input = @"-----------------------------------------------------
93
2018-07-07
2018-07-07
Opłata
-5,00
PLN
271,87
Opłata za Powiadomienia SMS 
Data waluty: 2018-07-07
-----------------------------------------------------";
            var userStock = new Stock { Name = "Intelligo bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 7, 7);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            decimal balance = 271.87m;
            string title = @"93 Opłata";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"93 - Opłata za Powiadomienia SMS Data waluty: 2018-07-07 Opłata saldo: {balance.ToString(Strings.ValueFormat)} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 5m }
                    }
                }, userStock, externalStock);
            var parser = new IntelligoBankParser();

            //when
            var results = parser.Parse(input, userStock, externalStock, outcomeType, null);

            //then
            foreach (var result in results) ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balances.First().Value.Value);
        }

        [Fact]
        public void LongUncommonOutcomeParseTest()
        {
            //given
            string input = @"-----------------------------------------------------
120
2018-08-21
2018-08-19
Płatność kartą
-179,91
PLN
209,38
Lokalizacja:
Kraj: WIELKA BRYTANIA
Miasto: CDK2156
Adres: CDKEYS.COM
Data wykonania: 2018-08-19 00:00:00
Numer referencyjny: 11111218231005349661663
Oryginalna kwota operacji: 35.99 GBP
Data przetworzenia: 2018-08-20
Numer karty: * *111 
Data waluty: 2018-08-19
-----------------------------------------------------
-----------------------------------------------------";
            var userStock = new Stock { Name = "Intelligo bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 8, 19);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            decimal balance = 209.38m;
            string title = @"120 Płatność kartą";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"120 - Lokalizacja: Kraj: WIELKA BRYTANIA Miasto: CDK2156 Adres: CDKEYS.COM Data wykonania: 2018-08-19 00:00:00 Numer referencyjny: 11111218231005349661663 Oryginalna kwota operacji: 35.99 GBP Data przetworzenia: 2018-08-20 Numer karty: * *111 Data waluty: 2018-08-19 Płatność kartą saldo: {balance.ToString(Strings.ValueFormat)} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 179.91m }
                    }
                }, userStock, externalStock);
            var parser = new IntelligoBankParser();

            //when
            var results = parser.Parse(input, userStock, externalStock, outcomeType, null);

            //then
            foreach (var result in results) ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balances.First().Value.Value);
        }

        [Fact]
        public void MultipleTransactionsWithDifferentLenghtParseTest()
        {
            //given
            string input = @"133
2018-09-07
2018-09-07
Przelew z rachunku
-50,00
PLN
4 521,61
Dane adr. rach. przeciwst.:
PKO BP FINAT SP. Z O.O.
Tytuł: DOŁADOWANIE TELEFONU +48 111168049 T-MOBILE IDENTYFIKATOR OPERACJI: 53642285 
Data waluty: 2018-09-07 
134
2018-09-07
2018-09-07
Opłata
-5,00
PLN
4 516,61
Opłata za Powiadomienia SMS 
Data waluty: 2018-09-07
135
2018-09-07
2018-09-07
Opłata
-5,00
PLN
4 516,61
Opłata za Powiadomienia SMS 
Data waluty: 2018-09-07";
            var userStock = new Stock { Name = "Intelligo bank" };
            var parser = new IntelligoBankParser();

            //when
            var results = parser.Parse(input, userStock, null, new TransactionType(), null);

            //then
            Assert.Equal(3, results.Length);
        }
    }
}