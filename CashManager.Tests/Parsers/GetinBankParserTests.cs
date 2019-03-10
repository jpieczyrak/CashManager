using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Logic.Parsers;
using CashManager.Properties;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class GetinBankParserTests : BaseParserTests
    {
        [Fact]
        public void OutcomeOldFormatParseTest()
        {
            //given
            string input = @"    06.09.2016 – PRZELEW WYCHODZĄCY
JĘDRZEJ PIECZYRAK – [Sierpień] Czynsz + media

-684,62 PLN";

            var parser = new GetinBankParser();

            var userStock = new Stock { Name = "Getin" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2016, 9, 6);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            var expected = new Transaction(outcomeType, creationDate, "[Sierpień] Czynsz + media",
                "JĘDRZEJ PIECZYRAK: PRZELEW WYCHODZĄCY (PLN)",
                new[]
                {
                    new Position
                    {
                        Title = "[Sierpień] Czynsz + media",
                        Value = new PaymentValue { GrossValue = 684.62m }
                    }
                }, userStock, externalStock);


            //when
            var output = parser.Parse(input, userStock, externalStock, outcomeType, null);
            var result = output.FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
        }

        [Fact]
        public void OutcomeNewFormatParseTest()
        {
            //given
            string input = @"
    25.10.2018 - Operacja kartą
CENTRUM NISKICH CEN SP, GLIWICE , PL

-47,07 PLN saldo po operacji: 2 735,57 PLN";

            var parser = new GetinBankParser();

            var userStock = new Stock { Name = "Getin" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 10, 25);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            string title = "CENTRUM NISKICH CEN SP, GLIWICE , PL";
            decimal balance = 2735.57m;
            var expected = new Transaction(outcomeType, creationDate, title,
                $"Operacja kartą (PLN) Saldo: {balance.ToString(Strings.ValueFormat)}",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 47.07m }
                    }
                }, userStock, externalStock);


            //when
            var output = parser.Parse(input, userStock, externalStock, outcomeType, null);
            var result = output.FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
        }

        [Fact]
        public void IncomeNewFormatParseTest()
        {
            //given
            string input = @"28.02.2014 – PRZELEW PRZYCHODZĄCY
Firma SP. Z O.O. – Wynagrodzenie z tytulu umowy cywilnoprawnej

+1 123,12 PLN saldo po operacji: 1 574,38 PLN";

            var parser = new GetinBankParser();

            var userStock = new Stock { Name = "Getin" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2014, 02, 28);
            var incomeType = new TransactionType { Income = true, Name = "Work" };
            string title = "Wynagrodzenie z tytulu umowy cywilnoprawnej";
            decimal balance = 1574.38m;
            var expected = new Transaction(incomeType, creationDate, title,
                $"Firma SP. Z O.O.: PRZELEW PRZYCHODZĄCY (PLN) Saldo: {balance.ToString(Strings.ValueFormat)}",
                new [] { new Position(title, 1123.12m) },
                userStock, externalStock);

            //when
            var output = parser.Parse(input, userStock, externalStock, null, incomeType);
            var result = output.FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
        }

        [Fact]
        public void DuplicateIncomeNewFormatParseTest_SingleTransaction()
        {
            //given
            string input = @"28.02.2014 – PRZELEW PRZYCHODZĄCY
Firma SP. Z O.O. – Wynagrodzenie z tytulu umowy cywilnoprawnej

+1 123,12 PLN saldo po operacji: 1 574,38 PLN

28.02.2014 – PRZELEW PRZYCHODZĄCY
Firma SP. Z O.O. – Wynagrodzenie z tytulu umowy cywilnoprawnej

+1 123,12 PLN saldo po operacji: 1 574,38 PLN";

            var parser = new GetinBankParser();

            var userStock = new Stock { Name = "Getin" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2014, 02, 28);
            var incomeType = new TransactionType { Income = true, Name = "Work" };
            string title = "Wynagrodzenie z tytulu umowy cywilnoprawnej";
            decimal balance = 1574.38m;
            var expected = new Transaction(incomeType, creationDate, title,
                $"Firma SP. Z O.O.: PRZELEW PRZYCHODZĄCY (PLN) Saldo: {balance.ToString(Strings.ValueFormat)}",
                new [] { new Position(title, 1123.12m) },
                userStock, externalStock);

            //when
            var output = parser.Parse(input, userStock, externalStock, null, incomeType);
            var result = output.FirstOrDefault();

            //then
            Assert.Single(output.Distinct());
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balances.First().Value.OrderByDescending(x => x.Key).First().Value);
        }

        [Fact]
        public void TransactionWithoutBalance_BalanceIsBeingCalculated()
        {
            //given
            string input = @"
    08.03.2019 - Operacja kartą
ALDI Sp. z o.o. 118 , Gliwice , PL

-27,87 PLN saldo po operacji: -

    01.03.2019 - PRZELEW PRZYCHODZĄCY
PIECZYRAK JĘDRZEJ - Zwrot za internet

+49,00 PLN saldo po operacji: 354,93 PLN

    01.03.2019 - PRZELEW WYCHODZĄCY
JĘDRZEJ PIECZYRAK - [02.19] Jedzenie etc

-391,79 PLN saldo po operacji: 815,85 PLN

    25.02.2019 - PRZELEW PRZYCHODZĄCY
PIECZYRAK JĘDRZEJ - Zwrot za paliwo

+75,53 PLN saldo po operacji: 1 207,64 PLN";

            var parser = new GetinBankParser();

            var userStock = new Stock { Name = "Getin" };
            var externalStock = new Stock { Name = "Default" };
            var incomeType = new TransactionType { Income = true, Name = "Work" };
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            decimal expectedBalance = 327.06m;
            DateTime expectedDate = new DateTime(2019, 03, 08);

            //when
            var output = parser.Parse(input, userStock, externalStock, outcomeType, incomeType);

            //then
            Assert.Equal(4, output.Length);
            var newestBalance = parser.Balances.First().Value.OrderByDescending(x => x.Key).First();
            Assert.Equal(expectedBalance, newestBalance.Value);
            Assert.Equal(expectedDate, newestBalance.Key);
        }
    }
}