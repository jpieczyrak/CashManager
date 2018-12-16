using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Logic.Parsers;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class IdeaBankTests : BaseParserTests
    {
        [Fact]
        public void SimpleOutcomeParseTest()
        {
            //given
            string input = @"Opłata za kartę - 10/2018
27.11.2018
-8,00 PLN
1 665,36
trash scam";
            var userStock = new Stock { Name = "Idea bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 11, 27);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            decimal balance = 1665.36m;
            var expected = new Transaction(outcomeType, creationDate, "Opłata za kartę - 10/2018",
                $"Saldo: {balance:#,##0.00}",
                new[]
                {
                    new Position
                    {
                        Title = "Opłata za kartę - 10/2018",
                        Value = new PaymentValue { GrossValue = 8.0m }
                    }
                }, userStock, externalStock, input);
            var parser = new IdeaBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, outcomeType, null).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balance.Value);
        }

        [Fact]
        public void OutcomeParse_2Transactions_NoError2Transactions()
        {
            //given
            string input = @"

Opłata za kartę - 10/2018
27.11.2018
-8,00 PLN
1 665,36


Opłata za kartę - 10/2018
27.11.2018
-8,00 PLN
1 665,36

spam not valid trash";
            var userStock = new Stock { Name = "Idea bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 11, 27);
            var outcomeType = new TransactionType { Outcome = true, Name = "Buy" };
            decimal balance = 1665.36m;
            var expected = new Transaction(outcomeType, creationDate, "Opłata za kartę - 10/2018",
                $"Saldo: {balance:#,##0.00}",
                new[]
                {
                    new Position
                    {
                        Title = "Opłata za kartę - 10/2018",
                        Value = new PaymentValue { GrossValue = 8.0m }
                    }
                }, userStock, externalStock, input);
            var parser = new IdeaBankParser();

            //when
            var results = parser.Parse(input, userStock, externalStock, outcomeType, null);

            //then
            Assert.Equal(2, results.Length);
            foreach (var result in results) ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balance.Value);
        }

        [Fact]
        public void SimpleIncomeParseTest()
        {
            //given
            string input = @"Premia - Promocja Premiowanie za Bankowanie - 10.2018

02.11.2018
50,00 PLN

1 375,81

";
            var userStock = new Stock { Name = "Idea bank" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2018, 11, 2);
            var incomeType = new TransactionType { Income = true, Name = "Income" };
            string title = "Premia - Promocja Premiowanie za Bankowanie - 10.2018";
            decimal balance = 1375.81m;
            var expected = new Transaction(incomeType, creationDate, title,
                $"Saldo: {balance:#,##0.00}",
                new[]
                {
                    new Position
                    {
                        Title = title,
                        Value = new PaymentValue { GrossValue = 50.0m }
                    }
                }, userStock, externalStock, input);
            var parser = new IdeaBankParser();

            //when
            var result = parser.Parse(input, userStock, externalStock, null, incomeType).FirstOrDefault();

            //then
            ValidateTransaction(result, expected);
            Assert.Equal(balance, parser.Balance.Value);
        }
    }
}