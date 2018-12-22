using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Logic.Parsers;

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
            string title = @"Lokalizacja: Kraj: POLSKA Miasto: GLIWICE Adres: ADRES Data wykonania: 2018-12-02 00:00:00 Numer referencyjny: 12312312312312312313123 Oryginalna kwota operacji: 12.34 PLN Numer karty: * *123 Data waluty: 2018-12-02";
            double balance = 9876.6;
            var expected = new Transaction(outcomeType, creationDate, title,
                $"Płatność kartą saldo: {balance:#,##0.00} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 12.34m }
                    }
                }, userStock, externalStock, input);
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
            string title = @"Lokalizacja: Kraj: POLSKA Miasto: GLIWICE Adres: ADRES Data wykonania: 2018-12-02 00:00:00 Numer referencyjny: 12312312312312312313123 Oryginalna kwota operacji: 12.34 PLN Numer karty: * *123 Data waluty: 2018-12-02";
            var expected = new Transaction(outcomeType, creationDate, title,
                $"Płatność kartą saldo: {9876.6:#,##0.00} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 12.34m }
                    }
                }, userStock, externalStock, input);
            var parser = new IntelligoBankParser();

            //when
            var results = parser.Parse(input, userStock, externalStock, outcomeType, null);

            //then
            foreach (var result in results) ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balance.Value);
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
            string title = @"Nr rach. przeciwst.: 00 1111 2222 3333 4444 5555 6666 Dane adr. rach. przeciwst.: SUPER FIRMA SP. Z O.O. UL. imię nazwisko 321 44-100 GLIWICE Tytuł: Wyplata: ETAT/2018/11/abc Data waluty: 2018-12-06";
            decimal balance = 10876.65m;
            var expected = new Transaction(incomeType, creationDate, title,
                $"Przelew na rachunek saldo: {balance:#,##0.00} (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 1000.05m }
                    }
                }, userStock, externalStock, input);
            var parser = new IntelligoBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, null, incomeType).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balance.Value);
        }
    }
}