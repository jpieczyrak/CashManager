﻿using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data;
using CashManager.Data.DTO;
using CashManager.Logic.Parsing;

using Xunit;

namespace CashManager.Tests.Parsing
{
    public class GetinBankParserTests
    {
        [Fact]
        public void OutcomeParseTest()
        {
            //given
            string input = @"    06.09.2016 – PRZELEW WYCHODZĄCY
JĘDRZEJ PIECZYRAK – [Sierpień] Czynsz + media

-684,62 PLN";

            var parser = new GetinBankParser();

            var userStock = new Stock { Name = "Getin" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2016, 9, 6);
            var expected = new Transaction(eTransactionType.Buy, creationDate, "[Sierpień] Czynsz + media",
                "JĘDRZEJ PIECZYRAK: PRZELEW WYCHODZĄCY (PLN)",
                new List<Position>
                {
                    new Position()
                    {
                        Title = "[Sierpień] Czynsz + media",
                        Value = new PaymentValue() { Value = 684.62d }
                    }
                }, userStock, externalStock, input);


            //when
            var output = parser.Parse(input, userStock, externalStock);
            var parsed = output.FirstOrDefault();

            //then
            Assert.NotNull(parsed);
            Assert.Equal(expected.BookDate, parsed.BookDate);
            Assert.Equal(expected.Title, parsed.Title);
            Assert.Equal(expected.Note, parsed.Note);
            Assert.Equal(expected.UserStock, parsed.UserStock);
            Assert.Equal(expected.ExternalStock, parsed.ExternalStock);

            Assert.Equal(creationDate, parsed.BookDate);
            Assert.Equal(creationDate, parsed.TransationSourceCreationDate);

			var instanceCreationDiff = expected.InstanceCreationDate - parsed.InstanceCreationDate;
			Assert.InRange(instanceCreationDiff, TimeSpan.FromSeconds(-1), TimeSpan.FromSeconds(1));
            
            Assert.Equal(expected.Positions.First().Title, parsed.Positions.First().Title);
            Assert.Equal(expected.Positions.First().Value.Value, parsed.Positions.First().Value.Value);

            Assert.Equal(expected.Positions.Sum(x => x.Value.Value), parsed.Positions.Sum(x => x.Value.Value));
        }

        [Fact]
        public void IncomeParseTest()
        {
            //given
            string input = @"28.02.2014 – PRZELEW PRZYCHODZĄCY
Firma SP. Z O.O. – Wynagrodzenie z tytulu umowy cywilnoprawnej

+1 123,12 PLN saldo po operacji: 1 574,38 PLN";

            var parser = new GetinBankParser();
            
            var userStock = new Stock { Name = "Getin" };
            var externalStock = new Stock { Name = "Default" };
            var creationDate = new DateTime(2014, 02, 28);
            var expected = new Transaction(eTransactionType.Work, creationDate, "Wynagrodzenie z tytulu umowy cywilnoprawnej",
                "Firma SP. Z O.O.: PRZELEW PRZYCHODZĄCY (PLN)",
                new List<Position>
                {
                    new Position("Wynagrodzenie z tytulu umowy cywilnoprawnej", 1123.12d)
                }, userStock, externalStock, input);

            //when
            var output = parser.Parse(input, userStock, externalStock);
            var parsed = output.FirstOrDefault();

            //then
            Assert.NotNull(parsed);
            Assert.Equal(expected.BookDate, parsed.BookDate);
            Assert.Equal(expected.Title, parsed.Title);
            Assert.Equal(expected.Note, parsed.Note);
            Assert.Equal(expected.UserStock, parsed.UserStock);
            Assert.Equal(expected.ExternalStock, parsed.ExternalStock);

            Assert.Equal(creationDate, parsed.BookDate);
            Assert.Equal(creationDate, parsed.TransationSourceCreationDate);

            Assert.Equal(expected.Positions.First().Title, parsed.Positions.First().Title);
            Assert.Equal(expected.Positions.First().Value.Value, parsed.Positions.First().Value.Value);

            Assert.Equal(expected.Positions.Sum(x => x.Value.Value), parsed.Positions.Sum(x => x.Value.Value));
        }
    }
}